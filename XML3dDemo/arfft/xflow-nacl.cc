
#include "ppapi/cpp/instance.h"
#include "ppapi/cpp/module.h"
#include "ppapi/cpp/var.h"
#include "ppapi/cpp/var_array_buffer.h"
#include "ppapi/cpp/var_dictionary.h"


namespace xflow {


    class NaclInstance : public pp::Instance
    {
    public:
        explicit NaclInstance(PP_Instance instance)
          : pp::Instance(instance),
            m_transform(16*sizeof(float)),
            m_perspective(16*sizeof(float)),
            m_videoWidth(-2)
        {
            // TODO: initialize SSTT
            mX=mY=0.0f;
        }

        virtual ~NaclInstance()
        {
            // TODO: cleanup SSTT
        }

        virtual void HandleMessage(const pp::Var& var_message)
        {
            if (var_message.is_array_buffer()) {

                processVideoFrame(var_message);

            } else {

                parseMessage(var_message);

            }
        }

    protected:
        void processVideoFrame(const pp::Var& var_message)
        {
            pp::VarArrayBuffer buffer(var_message);
            uint32_t l = buffer.ByteLength();

            if (l != 4*m_videoWidth*m_videoHeight) {
                // TODO: error handling
            }
            
            if (m_videoWidth*m_videoHeight==0) return;

            uint32_t* s = static_cast<uint32_t*>(buffer.Map());
            
            ComputeMoment(s,m_videoWidth,m_videoHeight);
        

            // TODO: provide current frame to SSTT and update m_transform/m_perspective/visibility accordingly

            float* t = static_cast<float*>(m_transform.Map());
            float* p = static_cast<float*>(m_perspective.Map());

            // transform:
/*            t[ 0] = -0.89442718029022220f; t[ 4] =  0.0f;                t[ 8] = -0.44721359014511110f; t[12] =   0.0f;
            t[ 1] = -0.22903932631015778f; t[ 5] =  0.8588975071907043f; t[ 9] =  0.45807865262031555f; t[13] =   0.0f;
            t[ 2] =  0.38411062955856323f; t[ 6] =  0.5121475458145142f; t[10] = -0.76822125911712650f; t[14] = -15.620499610900879f;
            t[ 3] =  0.0f;                 t[ 7] =  0.0f;                t[11] =  0.0f;                 t[15] =   1.0f;*/
            t[ 0] =  1.0f;                 t[ 4] =  0.0f;                t[ 8] =  0.0f;                 t[12] =   mX*16.0f;
            t[ 1] =  0.0f;                 t[ 5] =  1.0f;                t[ 9] =  0.0f;                 t[13] =   mY*12.0f;
            t[ 2] =  0.0f;                 t[ 6] =  0.0f;                t[10] =  1.0f;                 t[14] =   -16.0f;
            t[ 3] =  0.0f;                 t[ 7] =  0.0f;                t[11] =  0.0f;                 t[15] =   1.0f;

            // perspective:
            p[ 0] =  1.91013443470001220f; p[ 4] =  0.0f;                p[ 8] =  0.0f;                 p[12] =   0.0f;
            p[ 1] =  0.0f;                 p[ 5] =  2.5404789447784424f; p[ 9] =  0.0f;                 p[13] =   0.0f;
            p[ 2] =  0.0f;                 p[ 6] =  0.0f;                p[10] = -1.00133419036865230f; p[14] =  -0.20013342797756195f;
            p[ 3] =  0.0f;                 p[ 7] =  0.0f;                p[11] = -1.0f;                 p[15] =   0.0f;

            bool visibility = true;

            m_transform.Unmap();
            m_perspective.Unmap();

            buffer.Unmap();
        
            m_result.Set(pp::Var("transform"), m_transform);
            m_result.Set(pp::Var("perspective"), m_perspective);
            m_result.Set(pp::Var("visibility"), pp::Var(visibility));

            PostMessage(m_result);
        }

        void parseMessage(const pp::Var& var_message);

        template<typename T>
        bool parse(const pp::Var& v, T& t) {
            return false;
        }

    private:
        
        struct Color { uint8_t r,g,b,a; };

        void ComputeMoment(uint32_t* src, uint32_t width, uint32_t height)
        {
            uint32_t sumX=0, sumY[width], hitYArray[width];
            uint32_t hitX=0, hitY=0;
            float volX=(float)(width*(width+1))/2.0f;
            float volY=(float)(height*(height+1))/2.0f;
            
            float avgX=0.0f;
            float avgY=0.0f;
            for (int i = 0; i < width; i++)
            {
                sumY[i]=0;
                hitYArray[i]=0;
            }
            
            for (int j = 0; j < height; j++)
            {
                hitX=0;
                sumX=0;
                for (int i = 0; i < width; i++)
                {
                    const Color& cc = *(Color*)&src[i+j*width];
                    if (Detect(cc))
                    {
                        sumX+=i; hitX++;
                        sumY[i]+=j; hitYArray[i]++;
                    }
                }
                if (hitX)
                {
                    avgX+=((float)sumX)/(float)(hitX*width);
                    hitY++;
                }
            }
            if (hitY)
                avgX/=(float)(hitY);

            hitX=0;
            for (int i = 0; i < width; i++)
            {
                if (hitYArray[i])
                {
                    avgY+=((float)sumY[i])/(float)(hitYArray[i]*height);
                    hitX++;
                }
            }
            if (hitX)
                avgY/=(float)(hitX);
        
            mX=avgX-0.5f;
            mY=0.5-avgY;
            if (mX>0.5f) mX=0.5f;
            if (mY<-0.5f) mY=-0.5f;
        }
        
        bool Detect(const Color& c)
        {
            return (c.r > 252 && c.g > 252 && c.b > 252);
                
                //float i = c.r - 0.5f * (c.g + c.b);
                //return (i > 0.6f);
        }

    private:

        float mX, mY;
        int32_t m_videoWidth, m_videoHeight;
        pp::VarDictionary m_result;
        pp::VarArrayBuffer m_transform;
        pp::VarArrayBuffer m_perspective;
    };


    template<>
    bool NaclInstance::parse<int32_t>(const pp::Var& v, int32_t& t) {
        if (!v.is_int())
            return false;
        t = v.AsInt();
        return true;
    }


    void NaclInstance::parseMessage(const pp::Var& var_message)
    {
        // TODO: parse message
        pp::VarDictionary update(var_message);
        std::string op = update.Get(pp::Var("message")).AsString();

        if (op == "videoresize") {

            parse(update.Get(pp::Var("width")), m_videoWidth);
            parse(update.Get(pp::Var("height")), m_videoHeight);

            // TODO: recalculate perspective matrix

        } else if (op == "addtarget") {

            // TODO: extract marker image array buffer
            int32_t w, h;
            
            parse(update.Get(pp::Var("width")), w);
            parse(update.Get(pp::Var("height")), h);
            
            pp::VarArrayBuffer buffer(update.Get(pp::Var("data")));
            uint32_t l = buffer.ByteLength();
            uint8_t* s = static_cast<uint8_t*>(buffer.Map());

            // TODO: provide new tracking target to SSTT

            buffer.Unmap();
        }
    }


    ///////////////////////////////////////////////////////////////////////////

    class NaclModule : public pp::Module
    {
    public:
        NaclModule() : pp::Module() {}
        virtual ~NaclModule() {}

        virtual pp::Instance* CreateInstance(PP_Instance instance) {
            return new NaclInstance(instance);
        }
    };

} // namespace xflow


///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////


namespace pp {

    Module* CreateModule() {
        return new xflow::NaclModule();
    }

}  // namespace pp


