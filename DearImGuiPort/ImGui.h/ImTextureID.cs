
namespace ImGui
{
    // user data to identify a texture (this is whatever to you want it to be! read the FAQ about ImTextureID in imgui.cpp)
    public class ImTextureID
    {
        public object Data { get; private set; }
        public ImTextureID(object data)
        {
            Data = data;
        }
    }
}
