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
        private static Texture? _textureBrick;
        private static Texture? _textureDesert;

        public static Font FontCourerNew
        {
            get
            {
                if (_fontCourerNew == null)
                {
                    _fontCourerNew = new Font(Path.Combine(Directory.GetCurrentDirectory(), "Resources/cour.ttf"));
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
                    _textureSky = new Texture(Path.Combine(Directory.GetCurrentDirectory(), "Resources/sky.jpg"));
                }
                return _textureSky;
            }
        }
        public static string BrickPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources/brickWall1200.jpg");
        public static Texture TextureBrick
        {
            get
            {
                if (_textureBrick == null)
                {
                    var filename = Path.Combine(Directory.GetCurrentDirectory(), "Resources/1nf_flash_akkord_coral.jpg");
                    _textureBrick = new Texture(filename);
                }
                return _textureBrick;
            }
        }

        public static Texture TextureDesert
        {
            get
            {
                if (_textureDesert == null)
                {
                    var filename = Path.Combine(Directory.GetCurrentDirectory(), "Resources/desert1200.jpg");
                    _textureDesert = new Texture(filename);
                }
                return _textureDesert;
            }
        }
    }
}
