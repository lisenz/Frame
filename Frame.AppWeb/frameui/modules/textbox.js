/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.13
* 说明：TextBox 文本框控件。
*/
(function ($) {
    $.fn.TextBox = function (options) {
        return $.Frame.Do.call(this, "TextBox", arguments);
    };

    $.Methods.TextBox = $.Methods.TextBox || {};

    $.Defaults.TextBox = {
        Icon: null,
        Size: "medium",   // mini,small,medium,large,xlarge,xxlarge
        IconSite: "left", // left,right
        IconColor: "blue",
        Value: null,
        ReadOnly: false,
        Disabled: false,
        CanMultiLine: false,
        AutoSize: false,
        Width: null,
        Height: null,
        OnBeforeFocus: null,
        OnFocus: null,
        OnBlur: null,
        OnChange: null
    };

    $.Frame.Controls.TextBox = function (element, options) {
        $.Frame.Controls.TextBox.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.TextBox.Extension($.Frame.Controls.Base, {
        _GetType: function () {
            return "TextBox";
        },
        _IdPrev: function () {
            return "TextBoxFor";
        },
        _ExtendMethods: function () {
            return $.Methods.TextBox;
        },
        _PreRender: function () {
            var textbox = this;

            if (!$(textbox.Element).hasClass("frame-textbox")) {
                $(textbox.Element).addClass("frame-textbox");
            }
        },
        _Render: function () {
            var textbox = this, options = this.Options;

            $(textbox.Element).on("focus.TextBox", function () {
                var ifcontinue = true;
                if (textbox.HasBind("BeforeFocus"))
                    ifcontinue = textbox.Trigger("BeforeFocus", [this]);
                if (ifcontinue) {
                    if (textbox.HasBind("Focus"))
                        textbox.Trigger("Focus", [this]);
                }
                return false;
            }).on("blur.TextBox", function () {
                if (textbox.HasBind("Blur"))
                    textbox.Trigger("Blur", [this]);
                return false;
            }).on("keyup.TextBox", function () {
                if (textbox.HasBind("Change"))
                    textbox.Trigger("Change", [this.value]);
                return false;
            });

            textbox.Set(options);
        },
        _SetIcon: function (icon) {
            var textbox = this, options = this.Options;

            // TODO:文本框不能是多行文本框，该属性才能生效
            if (icon && !options.CanMultiLine) {
                textbox.Wrapper = $("<div></div>").addClass("input-icon");
                if (options.IconSite == "right")
                    textbox.Wrapper.addClass("input-icon-right");

                $(textbox.Element).wrap(textbox.Wrapper);
                $("<i></i>").addClass(icon).addClass(options.IconColor).insertBefore(textbox.Element);

            }
        },
        _SetSize: function (size) {
            ///<summary>
            /// 设置文本框的尺寸样式
            ///</summary>
            ///<param name="size" type="string">尺寸类型.[mini,small,medium,large,xlarge,xxlarge]</param>
            var textbox = this, options = this.Options;
            if (size) {
                if (textbox.Wrapper) {
                    $(textbox.Element).parent().addClass("input-" + size);
                    $(textbox.Element).addClass("input-" + size);
                }
                else
                    $(textbox.Element).addClass("input-" + size);
            }
        },
        _SetValue: function (value) {
            var textbox = this, options = this.Options;
            if (value) {
                $(textbox.Element).val(value);
            }
        },
        _GetValue: function () {
            var textbox = this, options = this.Options;
            return $(textbox.Element).val();
        },
        _SetReadOnly: function (readonly) {
            var textbox = this, options = this.Options;
            if (readonly != null) {
                if (readonly)
                    $(textbox.Element).attr("readonly", "readonly");
                else
                    $(textbox.Element).removeAttr("readonly");
            }
        },
        _GetReadOnly: function () {
            var textbox = this, options = this.Options;
            return $(textbox.Element).attr("readonly");
        },
        _SetDisabled: function (disabled) {
            var textbox = this, options = this.Options;
            if (disabled != null) {
                if (disabled)
                    $(textbox.Element).attr("disabled", "disabled");
                else
                    $(textbox.Element).removeAttr("disabled");
            }
        },
        _GetDisabled: function () {
            var textbox = this, options = this.Options;
            return $(textbox.Element).attr("disabled");
        },
        _SetWidth: function (width) {
            var textbox = this, options = this.Options;
            if (width) {
                if (textbox.Wrapper){
                    $(textbox.Element).parent().width(width);
                }
                $(textbox.Element).width(width);
            }
        },
        _SetHeight: function (height) {
            ///<summary>
            /// 设置文本框高度
            ///</summary>
            ///<param name="height" type="number">文本框的高度。这里只有当文本框为多行文本框时才能设置生效。</param>
            var textbox = this, options = this.Options;
            if (height && options.CanMultiLine)
                $(textbox.Element).height(height);
        },
        _SetAutoSize: function (auto) {
            var textbox = this, options = this.Options;
            if (auto && options.CanMultiLine) {
                $(textbox.Element).autosize();
            }
        },
        Focus: function () {
            ///<summary>
            /// 设置文本框焦点
            ///</summary>
            var textbox = this, options = this.Options;
            $(textbox.Element).trigger("focus");

            var pos = textbox.Element.value.length;
            var textRange = textbox.Element.createTextRange();
            textRange.collapse(true);
            textRange.moveEnd("character", pos);
            textRange.moveStart("character", pos);
            textRange.select();
        },
        Readable: function () {
            ///<summary>
            /// 设置文本框可读写
            ///</summary>
            var textbox = this, options = this.Options;
            textbox.Set("ReadOnly", false);
        },
        UnReadable: function () {
            ///<summary>
            /// 设置文本框只读
            ///</summary>
            var textbox = this, options = this.Options;
            textbox.Set("ReadOnly", true);
        }
    });

})(jQuery);

/*!
Autosize v1.17.1 - 2013-06-23
Automatically adjust textarea height based on user input.
(c) 2013 Jack Moore - http://www.jacklmoore.com/autosize
license: http://www.opensource.org/licenses/mit-license.php

--用于将TextArea多行文本框设置随文本内容自动拓展高度
*/
(function (e) {
    var t, o = {
        className: "autosizejs",
        append: "",
        callback: !1,
        resizeDelay: 10
    }, i = '<textarea tabindex="-1" style="position:absolute; top:-999px; left:0; right:auto; bottom:auto; border:0; -moz-box-sizing:content-box; -webkit-box-sizing:content-box; box-sizing:content-box; word-wrap:break-word; height:0 !important; min-height:0 !important; overflow:hidden; transition:none; -webkit-transition:none; -moz-transition:none;"/>', n = ["fontFamily", "fontSize", "fontWeight", "fontStyle", "letterSpacing", "textTransform", "wordSpacing", "textIndent"], s = e(i).data("autosize", !0)[0];
    s.style.lineHeight = "99px", "99px" === e(s).css("lineHeight") && n.push("lineHeight"),
    s.style.lineHeight = "", e.fn.autosize = function (i) {
        return i = e.extend({}, o, i || {}), s.parentNode !== document.body && e(document.body).append(s),
        this.each(function () {
            function o() {
                var o, a = {};
                if (t = u, s.className = i.className, l = parseInt(h.css("maxHeight"), 10), e.each(n, function (e, t) {
                    a[t] = h.css(t);
                }), e(s).css(a), "oninput" in u) {
                    var r = u.style.width;
                    u.style.width = "0px", o = u.offsetWidth, u.style.width = r;
                }
            }
            function a() {
                var n, a, r, c;
                t !== u && o(), s.value = u.value + i.append, s.style.overflowY = u.style.overflowY,
                a = parseInt(u.style.height, 10), "getComputedStyle" in window ? (c = window.getComputedStyle(u),
                r = u.getBoundingClientRect().width, e.each(["paddingLeft", "paddingRight", "borderLeftWidth", "borderRightWidth"], function (e, t) {
                    r -= parseInt(c[t], 10);
                }), s.style.width = r + "px") : s.style.width = Math.max(h.width(), 0) + "px", s.scrollTop = 0,
                s.scrollTop = 9e4, n = s.scrollTop, l && n > l ? (u.style.overflowY = "scroll",
                n = l) : (u.style.overflowY = "hidden", d > n && (n = d)), n += p, a !== n && (u.style.height = n + "px",
                w && i.callback.call(u, u));
            }
            function r() {
                clearTimeout(c), c = setTimeout(function () {
                    h.width() !== z && a();
                }, parseInt(i.resizeDelay, 10));
            }
            var l, d, c, u = this, h = e(u), p = 0, w = e.isFunction(i.callback), f = {
                height: u.style.height,
                overflow: u.style.overflow,
                overflowY: u.style.overflowY,
                wordWrap: u.style.wordWrap,
                resize: u.style.resize
            }, z = h.width();
            h.data("autosize") || (h.data("autosize", !0), ("border-box" === h.css("box-sizing") || "border-box" === h.css("-moz-box-sizing") || "border-box" === h.css("-webkit-box-sizing")) && (p = h.outerHeight() - h.height()),
            d = Math.max(parseInt(h.css("minHeight"), 10) - p || 0, h.height()), h.css({
                overflow: "hidden",
                overflowY: "hidden",
                wordWrap: "break-word",
                resize: "none" === h.css("resize") || "vertical" === h.css("resize") ? "none" : "horizontal"
            }), "onpropertychange" in u ? "oninput" in u ? h.on("input.autosize keyup.autosize", a) : h.on("propertychange.autosize", function () {
                "value" === event.propertyName && a();
            }) : h.on("input.autosize", a), i.resizeDelay !== !1 && e(window).on("resize.autosize", r),
            h.on("autosize.resize", a), h.on("autosize.resizeIncludeStyle", function () {
                t = null, a();
            }), h.on("autosize.destroy", function () {
                t = null, clearTimeout(c), e(window).off("resize", r), h.off("autosize").off(".autosize").css(f).removeData("autosize");
            }), a());
        });
    };
})(window.jQuery || window.Zepto);