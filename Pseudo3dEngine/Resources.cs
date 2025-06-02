using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine
{
    public static class Resources
    {
        public static uint ScreenWidth = 1200;
        public static uint ScreenHeight = 800;
        public static uint SkyHeight = ScreenHeight / 2;



        private static Font? _fontCourerNew;
        private static Texture? _textureSky;

        public static Font FontCourerNew
        {
            get
            {
                if (_fontCourerNew == null)
                {
                    _fontCourerNew = new Font(Path.Combine(Directory.GetCurrentDirectory(), "cour.ttf"));
                }
                return _fontCourerNew;
            }
        }
        public static Texture TextureSky
        {
            get
            {
                if (_textureSky == null)
                {
                    _textureSky = new Texture(Path.Combine(@"d:\Projects\Pseudo3dEngine.Net\Pseudo3dEngine\", "sky.jpg"));
                }
                return _textureSky;
            }
        }
    }
}
