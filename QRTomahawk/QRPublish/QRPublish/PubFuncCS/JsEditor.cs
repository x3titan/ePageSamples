using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TamPubWin1 {
    public class JsEditor {
        public string jsText = "";
        public ReservedWords reservedWords = new ReservedWords();
        public ReservedWords classWords = new ReservedWords();
        public ReservedWords propertyWords = new ReservedWords();
        private Timer timer1 = new Timer();

        public JsEditor() {
            reservedWords.Add("break");
            reservedWords.Add("delete");
            reservedWords.Add("function");
            reservedWords.Add("return");
            reservedWords.Add("typeof");
            reservedWords.Add("case");
            reservedWords.Add("do");
            reservedWords.Add("if");
            reservedWords.Add("switch");
            reservedWords.Add("var");
            reservedWords.Add("catch");
            reservedWords.Add("else");
            reservedWords.Add("in");
            reservedWords.Add("this");
            reservedWords.Add("void");
            reservedWords.Add("continue");
            reservedWords.Add("false");
            reservedWords.Add("instanceof");
            reservedWords.Add("throw");
            reservedWords.Add("while");
            reservedWords.Add("debugger");
            reservedWords.Add("finally");
            reservedWords.Add("new");
            reservedWords.Add("true");
            reservedWords.Add("with");
            reservedWords.Add("default");
            reservedWords.Add("for");
            reservedWords.Add("null");
            reservedWords.Add("try");

            classWords.Add("createPanel");
            classWords.Add("addText");
            classWords.Add("addImage");
            classWords.Add("addLink");
            classWords.Add("addSkin");
            classWords.Add("addEdit");
            classWords.Add("g5");
            classWords.Add("g6");
            classWords.Add("g7");
            classWords.Add("ee.pageUrl");
            classWords.Add("ee.pageRoot");
            classWords.Add("ee.showPanel");
            classWords.Add("ee.showAni");
            classWords.Add("ee.hideAllLayer");
            classWords.Add("ee.getControl");
            classWords.Add("ee.getEditText");
            classWords.Add("ee.setEditText");
            classWords.Add("ee.getText");
            classWords.Add("ee.setText");
            classWords.Add("ee.setTextColor");
            classWords.Add("ee.setVisible");
            classWords.Add("ee.setRect");
            classWords.Add("ee.setTableCellText");
            classWords.Add("ee.setTableData");
            classWords.Add("ee.clearTableData");
            classWords.Add("ee.post");
            classWords.Add("ee.postUrl");
            classWords.Add("eb.isEmpty");
            classWords.Add("eb.createDiv");
            classWords.Add("eb.setDivRect");
            classWords.Add("eb.getDivRect");
            classWords.Add("eb.createImg");
            classWords.Add("eb.setImgRect");
            classWords.Add("eb.setAlpha");
            classWords.Add("eb.setAni");
            classWords.Add("eb.initTableBody");
            classWords.Add("eb.restretchImg");
            classWords.Add("eb.getInterpolation");
            classWords.Add("eb.setDock");
            classWords.Add("eb.processDock");
            classWords.Add("eio.clear");
            classWords.Add("eio.appendByte");
            classWords.Add("eio.appendWord");
            classWords.Add("eio.appendInt32");
            classWords.Add("eio.appendString8");
            classWords.Add("eio.appendString16");
            classWords.Add("eio.appendString32");
            classWords.Add("eio.readByte");
            classWords.Add("eio.readWord");
            classWords.Add("eio.readInt32");
            classWords.Add("eio.readString8");
            classWords.Add("eio.readString16");
            classWords.Add("eio.readString32");
            classWords.Add("eio.readDataSet");
            classWords.Add("db.executeQuery");
            classWords.Add("db.executeNoneQuery");
            classWords.Add("db.encode");
            classWords.Add("db.read");
            classWords.Add("db.getString");
            classWords.Add("db.getInt32");
            classWords.Add("session.get");
            classWords.Add("session.set");
            classWords.Add("es.getMd5");
            classWords.Add("request.get");
            classWords.Add("response.redirect");

            //system
            classWords.Add("window.navigator");
            classWords.Add("window.screen");
            classWords.Add("window.history");
            classWords.Add("window.locatio");
            classWords.Add("window.document");
            classWords.Add("window.open");
            classWords.Add("window.close");
            classWords.Add("window.status");
            classWords.Add("window.defaultStatus");
            classWords.Add("window.alert");
            classWords.Add("window.confirm");
            classWords.Add("alert");
            classWords.Add("window.prompt");
            classWords.Add("window.location.href");
            classWords.Add("window.location.protocol");
            classWords.Add("window.location.hostname");
            classWords.Add("window.location.port");
            classWords.Add("window.location.host");
            classWords.Add("window.location.pathname");
            classWords.Add("window.location.reload");
            classWords.Add("window.focus");
            classWords.Add("window.blur");
            classWords.Add("window.showModaldialog");
            classWords.Add("window.showModeless");
            classWords.Add("window.onerror");
            classWords.Add("window.navigate");
            classWords.Add("window.print");
            classWords.Add("window.scroll");
            classWords.Add("window.scrollby");
            classWords.Add("window.onload");
            classWords.Add("window.onunload");
            classWords.Add("window.history.back");
            classWords.Add("window.history.forward");
            classWords.Add("window.history.go");
            classWords.Add("document.write");
            classWords.Add("document.getElementById");
            classWords.Add("parseInt");
            classWords.Add("parseFloat");
            classWords.Add("document.forms");
            classWords.Add("document.close");
            classWords.Add("document.createElement");
            classWords.Add("document.createTextNode");
            classWords.Add("document.getElementById");
            classWords.Add("document.all.tags");
            classWords.Add("document.write");
            classWords.Add("document.writeln");
            classWords.Add("document.body.noWrap");
            classWords.Add("Math.PI");
            classWords.Add("Math.SQRT2");
            classWords.Add("Math.max");
            classWords.Add("Math.min");
            classWords.Add("Math.pow");
            classWords.Add("Math.round");
            classWords.Add("Math.floor");
            classWords.Add("Math.random");
            classWords.Add("Date");
            classWords.Add("frames");
            classWords.Add("javascript");
            classWords.Add("Array");
            classWords.Add("Boolean");
            classWords.Add("Function");
            classWords.Add("String");
            classWords.Add("Object");
            classWords.Add("Number");
            classWords.Add("external.AddFavorite");
            classWords.Add("self");
            classWords.Add("setInterval");
            classWords.Add("setTimeout");
            classWords.Add("clearTimeout");

            propertyWords.Add("eeChildren");
            propertyWords.Add("length");
            propertyWords.Add("checked");
            propertyWords.Add("toUpperCase");
            propertyWords.Add("toLowerCase");
            propertyWords.Add("indexOf");
            propertyWords.Add("charAt");
            propertyWords.Add("charCodeAt");
            propertyWords.Add("substring");
            propertyWords.Add("concat");
            propertyWords.Add("lastIndexOf");
            propertyWords.Add("match");
            propertyWords.Add("replace");
            propertyWords.Add("split");
            propertyWords.Add("substr");
            propertyWords.Add("getTime");
            propertyWords.Add("getYear");
            propertyWords.Add("getFullYear");
            propertyWords.Add("getMonth");
            propertyWords.Add("getDate");
            propertyWords.Add("getDay");
            propertyWords.Add("getHours");
            propertyWords.Add("getMinutes");
            propertyWords.Add("getSeconds");
            propertyWords.Add("setTime");
            propertyWords.Add("setYear");
            propertyWords.Add("setMonth");
            propertyWords.Add("setDate");
            propertyWords.Add("setDay");
            propertyWords.Add("setHours");
            propertyWords.Add("setMinutes");
            propertyWords.Add("setSeconds");
            propertyWords.Add("parent");
            propertyWords.Add("top");
            propertyWords.Add("opener");
            propertyWords.Add("tabIndex");
            propertyWords.Add("innerHTML");
            propertyWords.Add("innerTEXT");
            propertyWords.Add("contentEditable");
            propertyWords.Add("isContentEditable");
            propertyWords.Add("isDisabled");
            propertyWords.Add("disabled");
            propertyWords.Add("select");
            propertyWords.Add("src");
            propertyWords.Add("onclick");
            propertyWords.Add("onmouseenter");
            propertyWords.Add("onmouseleave");
            propertyWords.Add("setAttribute");
            propertyWords.Add("appendChild");
            propertyWords.Add("push");
            propertyWords.Add("style");
            propertyWords.Add("style.left");
            propertyWords.Add("style.top");
            propertyWords.Add("style.width");
            propertyWords.Add("style.height");
            propertyWords.Add("style.scrolling");
            propertyWords.Add("style.overflow");
            propertyWords.Add("style.border");
            propertyWords.Add("style.position");
            propertyWords.Add("style.borderBottom");
            propertyWords.Add("style.borderBottomColor");
            propertyWords.Add("style.borderBottomStyle");
            propertyWords.Add("style.borderBottomWidth");
            propertyWords.Add("style.borderColor");
            propertyWords.Add("style.borderLeft");
            propertyWords.Add("style.borderLeftColor");
            propertyWords.Add("style.borderLeftStyle");
            propertyWords.Add("style.borderLeftWidth");
            propertyWords.Add("style.borderRight");
            propertyWords.Add("style.borderRightColor");
            propertyWords.Add("style.borderRightStyle");
            propertyWords.Add("style.borderRightWidth");
            propertyWords.Add("style.borderStyle");
            propertyWords.Add("style.borderTop");
            propertyWords.Add("style.borderTopColor");
            propertyWords.Add("style.borderTopStyle");
            propertyWords.Add("style.borderTopWidth");
            propertyWords.Add("style.borderWidth");
            propertyWords.Add("style.clear");
            propertyWords.Add("style.floatStyle");
            propertyWords.Add("style.margin");
            propertyWords.Add("style.marginBottom");
            propertyWords.Add("style.marginLeft");
            propertyWords.Add("style.marginRight");
            propertyWords.Add("style.marginTop");
            propertyWords.Add("style.padding");
            propertyWords.Add("style.paddingBottom");
            propertyWords.Add("style.paddingLeft");
            propertyWords.Add("style.paddingRight");
            propertyWords.Add("style.paddingTop");
            propertyWords.Add("style.background");
            propertyWords.Add("style.backgroundAttachment");
            propertyWords.Add("style.backgroundColor");
            propertyWords.Add("style.backgroundImage");
            propertyWords.Add("style.backgroundPosition");
            propertyWords.Add("style.backgroundRepeat");
            propertyWords.Add("style.color");
            propertyWords.Add("style.display");
            propertyWords.Add("style.listStyleType");
            propertyWords.Add("style.listStyleImage");
            propertyWords.Add("style.listStylePosition");
            propertyWords.Add("style.listStyle");
            propertyWords.Add("style.whiteSpace");
            propertyWords.Add("style.font");
            propertyWords.Add("style.fontFamily");
            propertyWords.Add("style.fontSize");
            propertyWords.Add("style.fontStyle");
            propertyWords.Add("style.fontVariant");
            propertyWords.Add("style.fontWeight");
            propertyWords.Add("style.letterSpacing");
            propertyWords.Add("style.lineBreak");
            propertyWords.Add("style.lineHeight");
            propertyWords.Add("style.textAlign");
            propertyWords.Add("style.textDecoration");
            propertyWords.Add("style.textIndent");
            propertyWords.Add("style.textJustify");
            propertyWords.Add("style.textTransform");
            propertyWords.Add("style.verticalAlign");
            propertyWords.Add("style.visibility");
            propertyWords.Add("style.cursor");

            timer1.Interval = 500;
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Enabled = true;
        }

        private RichTextBox frichTextBox = null;
        public RichTextBox richTextBox {
            get {
                return frichTextBox;
            }
            set {
                frichTextBox = value;
                if (value != null) {
                    //initialize richTextBox
                    value.LanguageOption = RichTextBoxLanguageOptions.DualFont;
                    value.AcceptsTab = true;
                    value.TextChanged += new EventHandler(richTextBox_TextChanged);
                    value.KeyDown += new KeyEventHandler(richTextBox_KeyDown);
                }
            }
        }

        void richTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Return) {
                e.SuppressKeyPress = true;
                string buff = richTextBox.Text;
                int sp = richTextBox.SelectionStart;
                int lineStart = 0;
                int spaceCount = 0;
                for (int i = sp - 1; i >= 0; i--) {
                    if (buff[i] == '\n') {
                        lineStart = i + 1;
                        break;
                    }
                }
                for (int i = lineStart; i < buff.Length; i++) {
                    if (buff[i] == ' ') {
                        spaceCount++;
                    } else if (buff[i] == '\t') {
                        spaceCount += 4;
                        spaceCount = (spaceCount / 4) * 4;
                    } else break;
                }
                string spaceString = "";
                for (int i = 0; i < spaceCount; i++) spaceString += " ";
                richTextBox.Text = buff.Substring(0, sp) + "\n" + spaceString + buff.Substring(sp);
                richTextBox.SelectionStart = sp + 1 + spaceString.Length;
                analyze();
            } else if (e.KeyCode == Keys.Tab) {
                e.SuppressKeyPress = true;
                string buff = richTextBox.Text;
                int sp = richTextBox.SelectionStart;
                int spaceCount = 0;
                for (int i = sp - 1; i >= 0; i--) {
                    if (buff[i] == '\n') break;
                    spaceCount++;
                }
                spaceCount = 4 - (spaceCount % 4);
                string spaceString = "";
                for (int i = 0; i < spaceCount; i++) {
                    spaceString += " ";
                }
                richTextBox.Text = buff.Substring(0, sp) + spaceString + buff.Substring(sp);
                richTextBox.SelectionStart = sp + spaceString.Length;
                analyze();
            }
        }

        private bool modified = true;
        void richTextBox_TextChanged(object sender, EventArgs e) {
            if (richTextBox.Text.IndexOf('\t') >= 0) {
                int s1 = richTextBox.SelectionStart;
                int s2 = richTextBox.SelectionLength;
                richTextBox.Text = convertTab(richTextBox.Text);
                richTextBox.SelectionStart = s1;
                richTextBox.SelectionLength = s2;
                analyze();
            } else {
                modified = true;
            }
        }

        private string convertTab(string buff) {
            StringBuilder result = new StringBuilder();
            int lineLen = 0, spaceCount;
            for (int i = 0; i < buff.Length; i++) {
                if (buff[i] == '\t') {
                    spaceCount = ((lineLen / 4) * 4 + 4) - lineLen;
                    for (int j = 0; j < spaceCount; j++) {
                        result.Append(' ');
                    }
                    lineLen += spaceCount;
                } else if (buff[i] == '\n') {
                    result.Append('\n');
                    lineLen = 0;
                } else {
                    result.Append(buff[i]);
                    lineLen++;
                }
            }
            return result.ToString();
        }

        private void analyze() {
            int selectionStart = richTextBox.SelectionStart;
            int selectionLen = richTextBox.SelectionLength;
            Point scrollPos = new Point();
            scrollPos.Y = DrawingControl.GetScrollPos(richTextBox.Handle, 1);
            scrollPos.X = DrawingControl.GetScrollPos(richTextBox.Handle, 0);
            DrawingControl.SuspendDrawing(richTextBox.Parent);
            int sp = 0;
            int lastSp;
            string s, classToken = "";
            string text = richTextBox.Text;
            while (true) {
                if (sp >= text.Length) break;
                lastSp = sp;
                if ("  \r\n".Contains(text[sp])) {
                    while (sp < text.Length) {
                        if (!"  \r\n".Contains(text[sp])) break;
                        sp++;
                    }
                } else if (TamPub1.StringFunc.copy(text, sp, 2).Equals("//")) {
                    classToken = "";
                    sp += 2;
                    while (sp < text.Length) {
                        if ("\r\n".Contains(text[sp])) break;
                        sp++;
                    }
                    setColor(lastSp, sp - lastSp, Color.FromArgb(0x00, 0x80, 0x00), false);
                } else if (TamPub1.StringFunc.copy(text, sp, 2).Equals("/*")) {
                    classToken = "";
                    sp += 2;
                    while (sp < text.Length) {
                        if (TamPub1.StringFunc.copy(text, sp, 2).Equals("*/")) {
                            sp += 2;
                            break;
                        }
                        sp++;
                    }
                    setColor(lastSp, sp - lastSp, Color.FromArgb(0x00, 0x80, 0x00), false);
                } else if (text[sp] == '"') {
                    classToken = "";
                    sp++;
                    while (sp < text.Length) {
                        if (TamPub1.StringFunc.copy(text, sp, 2).Equals("\\\"")) {
                            sp++;
                        } else if (text[sp] == '"') {
                            sp += 1;
                            break;
                        } else if ("\r\n".Contains(text[sp])) {
                            break;
                        }
                        sp++;
                    }
                    setColor(lastSp, sp - lastSp, Color.FromArgb(0x80, 0x00, 0x00), false);
                } else if (TamPub1.StringFunc.isLetter(text[sp])) {
                    s = TamPub1.StringFunc.getToken(text, ref sp, " \r\n.;+-=:,|()[]*/!@#$%^&");
                    classToken += s;
                    if (reservedWords.contains(s) && classToken.Equals(s)) {
                        setColor(sp - s.Length, s.Length, Color.FromArgb(0x00, 0x00, 0xff), true);
                    } else if (classWords.isClass(classToken)) {
                        setColor(sp - s.Length, s.Length, Color.FromArgb(0x00, 0x80, 0x80), false);
                    } else if (propertyWords.isProperty(classToken)) {
                        setColor(sp - s.Length, s.Length, Color.FromArgb(0x00, 0x80, 0x80), false);
                    } else {
                        setColor(sp - s.Length, s.Length, Color.FromArgb(0x00, 0x00, 0x00), false);
                    }
                } else if (text[sp] == '.') {
                    classToken += ".";
                    setColor(sp, 1, Color.FromArgb(0x00, 0x00, 0x00), false);
                    sp++;
                } else {
                    classToken = "";
                    setColor(sp, 1, Color.FromArgb(0x00, 0x00, 0x00), false);
                    sp++;
                }
            }

            richTextBox.SelectionStart = selectionStart;
            richTextBox.SelectionLength = selectionLen;
            richTextBox.Location = new Point(0, 10);
            DrawingControl.SetScrollPos(richTextBox.Handle, 1, scrollPos.Y, false);
            DrawingControl.SetScrollPos(richTextBox.Handle, 0, scrollPos.X, false);
            DrawingControl.SendMessage(richTextBox.Handle, DrawingControl.WM_HSCROLL, scrollPos.X * 65536 + 5, 0);
            DrawingControl.SendMessage(richTextBox.Handle, DrawingControl.WM_VSCROLL, scrollPos.Y * 65536 + 5, 0);
            DrawingControl.ResumeDrawing(richTextBox.Parent);
        }
        private void setColor(int start, int len, Color color, bool bold) {
            richTextBox.Select(start, len);
            richTextBox.SelectionColor = color;
            Font font;
            if (bold) {
                font = new Font("宋体", 9, FontStyle.Bold);
            } else {
                font = new Font("宋体", 9, FontStyle.Regular);
            }
            this.richTextBox.SelectionFont = font;
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (richTextBox == null) return;
            if (modified) {
                analyze();
                modified = false;
            }
        }
    }

    public class ReservedWords : List<string> {
        public bool contains(string value) {
            for (int i = 0; i < Count; i++) {
                if (this[i].Equals(value)) return true;
            }
            return false;
        }
        public bool isClass(string value) {
            for (int i = 0; i < Count; i++) {
                if (this[i].IndexOf(value) == 0) {
                    if (value.Length == this[i].Length) return true;
                    if (this[i][value.Length] == '.') return true;
                }
            }
            return false;
        }
        public bool isProperty(string value) {
            int a;
            for (int i = 0; i < Count; i++) {
                a = value.IndexOf(this[i]);
                if (a < 0) continue;
                if (a == value.Length - this[i].Length) {
                    if (!value.Equals(this[i])) return true;
                }
            }
            return false;
        }
    }

    class DrawingControl {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, Int32 wParam, Int32 lParam);

        public const int WM_SETREDRAW = 11;
        public const int WM_VSCROLL = 0x0115;
        public const int WM_HSCROLL = 0x0114;
        public static void SuspendDrawing(Control parent) {
            SendMessage(parent.Handle, WM_SETREDRAW, 0, 0);
        }

        public static void ResumeDrawing(Control parent) {
            SendMessage(parent.Handle, WM_SETREDRAW, 1, 0);
            parent.Refresh();
        }

        [DllImport("user32")]
        public static extern int GetScrollPos(IntPtr hwnd, int nBar);
        [DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, int nBar,
                                       int nPos, bool bRedraw);
    }
}
