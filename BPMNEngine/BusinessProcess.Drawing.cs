using BPMNEngine.Drawing;
using BPMNEngine.Interfaces.Elements;
using Microsoft.Maui.Graphics;
using System.Collections;
using System.Text;

namespace BPMNEngine
{
    public sealed partial class BusinessProcess
    {
        private static readonly TimeSpan ANIMATION_DELAY = new(0, 0, 1);
        private const float DEFAULT_PADDING = 100;
        private const int VARIABLE_NAME_WIDTH = 200;
        private const int VARIABLE_VALUE_WIDTH = 300;
        private const int VARIABLE_IMAGE_WIDTH = VARIABLE_NAME_WIDTH+VARIABLE_VALUE_WIDTH;

        /// <summary>
        /// Called to render a PNG image of the process
        /// </summary>
        /// <param name="type">The output image format to generate, this being jpeg,png or bmp</param>
        /// <returns>A Bitmap containing a rendered image of the process</returns>
        public byte[] Diagram(ImageFormat type)
            => Diagram(false)?.AsBytes(type);

        internal byte[] Diagram(bool outputVariables, ProcessState state, ImageFormat type)
            => Diagram(outputVariables, state: state)?.AsBytes(type);

        private IImage Diagram(bool outputVariables, ProcessState state = null)
        {
            state??=new ProcessState(null, this, null, null, null);
            WriteLogLine((IElement)null, LogLevel.Information, new StackFrame(1, true), DateTime.Now, string.Format("Rendering Business Process Diagram{0}", [(outputVariables ? " with variables" : " without variables")]));
            double width = 0;
            double height = 0;
            width = definition.Diagrams.Max(d => d.Size.Width+DEFAULT_PADDING);
            height = definition.Diagrams.Sum(d => d.Size.Height+DEFAULT_PADDING);
            IImage ret = null;
            try
            {
                var image = BPMNEngine.Elements.Diagram.ProduceImage((int)Math.Ceiling(width), (int)Math.Ceiling(height));
                var surface = image.Canvas;
                surface.FillColor=Colors.White;
                surface.FillRectangle(new Rect(0, 0, width, height));
                float padding = DEFAULT_PADDING / 2;
                definition.Diagrams.ForEach(d =>
                {
                    surface.DrawImage(d.Render(state.Path, this.definition), DEFAULT_PADDING / 2, padding, d.Size.Width, d.Size.Height);
                    padding += d.Size.Height + DEFAULT_PADDING;
                });
                ret = image.Image;
                if (outputVariables)
                    ret = AppendVariables(ret, state);
            }
            catch (Exception e)
            {
                WriteLogException((IElement)null, new StackFrame(1, true), DateTime.Now, e);
                ret=null;
            }
            return ret;
        }

        private static IImage ProduceVariablesImage(ProcessState state)
        {
            var image = BPMNEngine.Elements.Diagram.ProduceImage(1, 1);
            var canvas = image.Canvas;
            SizeF sz = canvas.GetStringSize("Variables", BPMNEngine.Elements.Diagram.DefaultFont, BPMNEngine.Elements.Diagram.FONT_SIZE);
            int varHeight = (int)sz.Height + 2;
            var keys = state[null];
            varHeight+=keys.Sum(key => (int)canvas.GetStringSize(key, BPMNEngine.Elements.Diagram.DefaultFont, BPMNEngine.Elements.Diagram.FONT_SIZE).Height + 2);

            image = BPMNEngine.Elements.Diagram.ProduceImage(VARIABLE_IMAGE_WIDTH, varHeight);
            var surface = image.Canvas;
            surface.FillColor = Colors.White;
            surface.FillRectangle(0, 0, image.Width, image.Height);

            surface.StrokeColor = Colors.Black;
            surface.StrokeDashPattern=null;
            surface.StrokeSize=1.0f;

            surface.DrawRectangle(0, 0, image.Width, image.Height);

            surface.DrawLine(new Point(0, (int)sz.Height + 2), new Point(VARIABLE_IMAGE_WIDTH, (int)sz.Height + 2));
            surface.DrawLine(new Point(VARIABLE_NAME_WIDTH, (int)sz.Height + 2), new Point(VARIABLE_NAME_WIDTH, image.Height));
            surface.DrawString("Variables", new Rect(0, 2, image.Width, sz.Height), HorizontalAlignment.Center, VerticalAlignment.Center);
            float curY = sz.Height + 2;
            keys.ForEach(key =>
            {
                string label = key;
                SizeF szLabel = canvas.GetStringSize(label, BPMNEngine.Elements.Diagram.DefaultFont, BPMNEngine.Elements.Diagram.FONT_SIZE);
                while (szLabel.Width > VARIABLE_NAME_WIDTH)
                {
                    if (label.EndsWith("..."))
                        label = string.Concat(label.AsSpan(0, label.Length - 4), "...");
                    else
                        label = string.Concat(label.AsSpan(0, label.Length - 1), "...");
                    szLabel = canvas.GetStringSize(label, BPMNEngine.Elements.Diagram.DefaultFont, BPMNEngine.Elements.Diagram.FONT_SIZE);
                }
                StringBuilder val = new();
                if (state[null, key] != null)
                {
                    if (state[null, key].GetType().IsArray)
                    {
                        ((IEnumerable)state[null, key]).Cast<object>().ForEach(o => val.AppendFormat("{0},", o));
                        val.Length--;
                    }
                    else if (state[null, key] is Hashtable hashtable)
                    {
                        val.Append('{');
                        hashtable.Keys.Cast<string>().ForEach(k => val.Append($"{{\"{k}\":\"{hashtable[k]}\"}},"));
                        val.Length--;
                        val.Append('}');
                    }
                    else
                        val.Append(state[null, key].ToString());
                }
                var sval = val.ToString();
                Size szValue = canvas.GetStringSize(sval, BPMNEngine.Elements.Diagram.DefaultFont, BPMNEngine.Elements.Diagram.FONT_SIZE);
                if (szValue.Width > VARIABLE_VALUE_WIDTH)
                {
                    if (sval.EndsWith("..."))
                        sval = string.Concat(sval.AsSpan(0, sval.Length - 4), "...");
                    else
                        sval = string.Concat(sval.AsSpan(0, sval.Length - 1), "...");
                    canvas.GetStringSize(sval, BPMNEngine.Elements.Diagram.DefaultFont, BPMNEngine.Elements.Diagram.FONT_SIZE);
                }
                surface.DrawString(label, 2, curY, HorizontalAlignment.Left);
                surface.DrawString(sval, 2+VARIABLE_NAME_WIDTH, curY, HorizontalAlignment.Left);
                curY += (float)Math.Max(szLabel.Height, szValue.Height) + 2;
                surface.DrawLine(new Point(0, curY), new Point(VARIABLE_IMAGE_WIDTH, curY));
            });
            return image.Image;
        }

        private static IImage AppendVariables(IImage diagram, ProcessState state)
        {
            var vmap = BusinessProcess.ProduceVariablesImage(state);
            var ret = BPMNEngine.Elements.Diagram.ProduceImage(
                (int)Math.Ceiling(diagram.Width + DEFAULT_PADDING + vmap.Width),
                (int)Math.Ceiling(Math.Max(diagram.Height, vmap.Height + DEFAULT_PADDING))
            );
            var surface = ret.Canvas;
            surface.FillColor = Colors.White;
            surface.FillRectangle(0, 0, ret.Width, ret.Height);
            surface.DrawImage(diagram, 0, 0, diagram.Width, diagram.Height);
            surface.DrawImage(vmap, diagram.Width + DEFAULT_PADDING, DEFAULT_PADDING, vmap.Width, vmap.Height);
            return ret.Image;
        }

        internal byte[] Animate(bool outputVariables, ProcessState state)
        {
            WriteLogLine((IElement)null, LogLevel.Information, new StackFrame(1, true), DateTime.Now, string.Format("Rendering Business Process Animation{0}", [(outputVariables ? " with variables" : " without variables")]));
            var result = Array.Empty<byte>();
            try
            {
                state.Path.StartAnimation();
                IImage bd = Diagram(false, state)??throw new DiagramException("Unable to create first diagram frame");
                AnimatedPNG apng = new((outputVariables ? AppendVariables(bd, state) : bd))
                {
                    DefaultFrameDelay= ANIMATION_DELAY
                };
                while (state.Path.HasNext())
                {
                    string nxtStep = state.Path.MoveToNextStep();
                    if (nxtStep != null)
                    {
                        double padding = DEFAULT_PADDING / 2;
                        if (definition!=null)
                        {
                            var diagram = definition.Diagrams.FirstOrDefault(d => d.RendersElement(nxtStep));
                            if (diagram!=null)
                            {
                                var rect = diagram.GetElementRectangle(nxtStep);
                                IImage img = diagram.Render(state.Path, definition, nxtStep);
                                apng.AddFrame(img, (int)Math.Ceiling((DEFAULT_PADDING / 2)+rect.X)+3, (int)Math.Ceiling(padding+rect.Y)+3);
                            }
                        }
                        if (outputVariables)
                            apng.AddFrame(ProduceVariablesImage(state), (int)Math.Ceiling(bd.Width + DEFAULT_PADDING), (int)DEFAULT_PADDING, delay: new TimeSpan(0, 0, 0));
                    }
                }
                state.Path.FinishAnimation();
                result = apng.ToBinary();
                apng.Dispose();
            }
            catch (Exception e)
            {
                WriteLogException((IElement)null, new StackFrame(1, true), DateTime.Now, e);
                result=null;
            }
            return result;
        }

    }
}
