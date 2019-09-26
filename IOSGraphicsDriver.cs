using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using NoZ.Platform.OpenGL;

namespace NoZ.Platform.IOS {
    class IOSGraphicsDriver : OpenGLDriver {
        private static Cursor _fakeCursor = new Cursor();

        public override Cursor CreateCursor(Image image) {
            return _fakeCursor;
        }

        public override Cursor CreateCursor(SystemCursor systemCursor) {
            return _fakeCursor;
        }

    }
}