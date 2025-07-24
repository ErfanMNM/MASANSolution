using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASAN_SERIALIZATION.Enums
{
    //trạng thái của ứng dụng
    public enum e_App_State
    {
        LOGIN,
        ACTIVE,
        DEACTIVE
    }

    //trạng thái giao diện của ứng dụng
    public enum e_App_Render_State
    {
        LOGIN,
        ACTIVE,
        DEACTIVE
    }

    public enum e_Camera_State
    {
        DISCONNECTED,
        CONNECTED,
        RECONNECTING
    }

}
