/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.11
* 说明：FormBox 窗体控件。
*/
(function ($) {
    $.fn.FormBox = function (options) {
        return $.Frame.Do.call(this, "FormBox", arguments);
    };

    $.Methods.FormBox = $.Methods.FormBox || {};

    $.Defaults.FormBox = {
        HasHeader: true,
        Closable: false,
        Commands: [],       //{Icon,OnClick}
        Title: "标题",
        Icon: "icon-desktop",
        HasFooter: true,
        Buttons: [],
        Left: "auto",
        Top: "auto",
        CloseOnDestroy: false,
        CanDraggable: false,
        CanResizable: false,
        IsModal: false,
        Url: null,
        Params: {},
        Content: null,
        Width: "auto",
        Height: "auto",
        OnBeforeShow: null,
        OnShown: null,
        OnBeforeClose: null,
        OnClose: null
    };

    $.Frame.Controls.FormBox = function (element, options) {
        $.Frame.Controls.FormBox.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.FormBox.Extension($.Frame.Controls.WindowBase, {
        _GetType: function () {
            return "FormBox";
        },
        _IdPrev: function () {
            return "FormBoxFor";
        },
        _ExtendMethods: function () {
            return $.Methods.FormBox;
        },
        _PreRender: function () {
            var formbox = this;

            //            if (self.frameElement && self.frameElement.tagName == "IFRAME") {
            //                $(formbox.Element).appendTo(parent.window.document.body);
            //            }
            //            else {
            //                $(formbox.Element).appendTo("body");
            //            }
            $(formbox.Element).appendTo("body");
            if (!$(formbox.Element).hasClass("frame-formbox")) {
                $(formbox.Element).addClass("frame-formbox");
            }

            if ($(formbox.Element).children().length > 0) {
                this.Options.Content = $(formbox.Element).children().first().detach();
                $(formbox.Element).empty();
            }

            formbox.MinHeight = 65;
        },
        _Render: function () {
            var formbox = this, options = this.Options;

            formbox.Container = $("<div></div>").addClass("frame-formbox-inner").appendTo(formbox.Element);
            formbox.Header = $("<div></div>").addClass("frame-formbox-header").appendTo(formbox.Container);
            formbox.Content = $("<div></div>").addClass("frame-formbox-content").appendTo(formbox.Container);
            formbox.Footer = $("<div></div>").addClass("frame-formbox-footer").appendTo(formbox.Container);

            formbox.Set(options);

            formbox._SetCenter();
            $(formbox.Element).hide();
        },
        _SaveStatus: function () {
            ///<summary>
            /// 保存记录当前面板位置及大小.用于当面板调用还原或显示方法时
            ///</summary>
            var formbox = this;
            formbox._Width = $(formbox.Element).width();
            formbox._Height = $(formbox.Element).height();
            var top = $(formbox.Element).offset().top;
            var left = $(formbox.Element).offset().left;
            if (!isNaN(parseInt($(formbox.Element).css("top"))))
                top = parseInt($(formbox.Element).css("top"));
            if (!isNaN(parseInt($(formbox.Element).css("left"))))
                left = parseInt($(formbox.Element).css("left"));
            formbox._Top = top;
            formbox._Left = left;
        },
        _SetHasHeader: function (hasHeader) {
            var formbox = this, options = this.Options;
            if (!hasHeader) {
                formbox.Header.remove();
            }
        },
        _SetClosable: function (closable) {
            var formbox = this, options = this.Options;
            if (closable && options.HasHeader) {
                var button = $("<button></button>").addClass("close").text("×").on("click", function () {
                    if (options.CloseOnDestroy)
                        formbox.Destroy();
                    else
                        formbox.Close();
                });
                button.appendTo(formbox.Header);
            }
        },
        _SetCommands: function (commands) {
            var formbox = this, options = this.Options;
            if (commands && commands.length > 0 && options.HasHeader) {
                var toolbar = $("<div></div>").addClass("frame-formbox-toolbar").appendTo(formbox.Header);
                $(commands).each(function () {
                    var command = this;
                    var btn = $("<a></a>").append($("<i></i>").addClass(command.Icon)).appendTo(toolbar);
                    if (command.OnClick) {
                        btn.on("click", function () {
                            command.OnClick.call(this, formbox);
                        });
                    }
                });
            }
        },
        _SetTitle: function (title) {
            var formbox = this, options = this.Options;
            if (title && options.HasHeader) {
                $("<h4></h4>").addClass("frame-formbox-title").addClass("blue").addClass("bigger")
                              .append($("<span></span>").text(title)).appendTo(formbox.Header);
            }
        },
        _SetIcon: function (icon) {
            var formbox = this, options = this.Options;
            if (icon && options.HasHeader) {
                $("h4", formbox.Header).prepend($("<i></i>").addClass(icon));
            }
        },
        _SetHasFooter: function (hasFooter) {
            var formbox = this, options = this.Options;
            if (!hasFooter) {
                formbox.Footer.remove();
            }
        },
        _SetButtons: function (buttons) {
            var formbox = this, options = this.Options;
            if (buttons && buttons.length > 0 && options.HasFooter) {
                $(buttons).each(function () {
                    var button = $("<button></button>").Button(this);
                    button.Context = formbox;
                    var orientation = this.Orientation == "right" ? "pull-right" : "pull-left";
                    formbox.Footer.append($(button.Element).addClass(orientation));
                });
            }
        },
        _SetWidth: function (width) {
            var formbox = this, options = this.Options;

            var newWidth = $(formbox.Element).parent().width() + 2;
            if (width != "auto") {
                newWidth = width;
                $(formbox.Element).width(newWidth);
            } else {
                $(formbox.Element).width(newWidth);
            }

            formbox.Content.width(newWidth - 12);
        },
        _SetHeight: function (height) {
            var formbox = this, options = this.Options;

            var newHeight = $(formbox.Element).parent().height() + 2;
            var headerOfHeight = (options.HasHeader && formbox.Header) ? formbox.Header.outerHeight(true) : 0;
            var footerOfHeight = (options.HasFooter && formbox.Footer) ? formbox.Footer.outerHeight(true) : 0;

            if (height != "auto") {
                newHeight = height - headerOfHeight - footerOfHeight + 2;
                if (newHeight < formbox.MinHeight)
                    newHeight = formbox.MinHeight + headerOfHeight + footerOfHeight;
                $(formbox.Element).height(newHeight);
                formbox.Content.height(newHeight - headerOfHeight - footerOfHeight - 13);
            } else {
                newHeight = formbox.Content.outerHeight(true);
                formbox.Content.height(newHeight - 10);
                $(formbox.Element).height(newHeight + headerOfHeight + footerOfHeight + 2);
            }
        },
        _SetLeft: function (left) {
            var formbox = this, options = this.Options;
            if (left != "auto") {
                $(formbox.Element).css("left", left);
            }
        },
        _SetTop: function (top) {
            var formbox = this, options = this.Options;
            if (top != "auto") {
                $(formbox.Element).css("top", top);
            }
        },
        _SetCanDraggable: function (can) {
            var formbox = this, options = this.Options;
            if (can && options.HasHeader) {
                formbox.Header.mousedown(function () {
                    $.Frame.WindowBase.SetFront(formbox);
                });
                formbox._ApplyDraggable();
            }
        },
        _ApplyDraggable: function () {
            ///<summary>
            /// 应用拖曳功能，使面板可进行拖动
            ///</summary>
            var formbox = this;
            if ($.fn.Draggable) {
                formbox.Draggable = $(formbox.Element).Draggable({
                    Handler: ".frame-formbox-title",
                    Animate: false,
                    OnStartDrag: function (e) {
                        $.Frame.WindowBase.SetFront(formbox);
                    },
                    OnStopDrag: function (e) {
                        formbox._SaveStatus();
                    }
                });
            }
        },
        _SetCanResizable: function (can) {
            var formbox = this, options = this.Options;
            if (can) {
                formbox._ApplyResizable();
            }
        },
        _ApplyResizable: function () {
            ///<summary>
            /// 应用调整尺寸功能，使面板可进行大小调整
            ///</summary>
            var formbox = this, options = this.Options;
            if ($.fn.Resizable) {
                formbox.Resizable = $(formbox.Element).Resizable({ Scope: 5,
                    OnStartResize: function (current, e) {
                        formbox._Deceive = formbox._Deceive || formbox._CreateDeceive();
                        formbox._Deceive.width($(formbox.Content).width() - 10).height($(formbox.Content).height() - 10);
                        $(formbox.Content).append(formbox._Deceive);
                    },
                    OnStopResize: function (current, e) {
                        var top = 0;
                        var left = 0;
                        var p = $(formbox.Element);
                        if (!options.HasHeader)
                            p.removeClass("frame-resizable");
                        if (!isNaN(parseInt(p.css("top"))))
                            top = parseInt(p.css("top"));
                        if (!isNaN(parseInt(p.css("left"))))
                            left = parseInt(p.css("left"));
                        if (current.DiffLeft) {
                            formbox.Set({ Left: left + current.DiffLeft });
                        }
                        if (current.DiffTop) {
                            formbox.Set({ Top: top + current.DiffTop });
                        }
                        if (current.NewWidth) {
                            formbox.Set({ Width: current.NewWidth });
                        }
                        if (current.NewHeight) {
                            formbox.Set({ Height: current.NewHeight });
                        }
                        formbox._SaveStatus();
                        formbox._RemoveDeceive();
                        return false;
                    }
                });
            }
        },
        _CreateDeceive: function () {
            ///<summary>
            /// 创建蒙层
            ///</summary>            
            return $("<div></div>").css("display", "block")
                                    .css("overflow", "hidden")
                                    .css("border", "0")
                                    .css("opacity", "0")
                                    .css("position", "absolute")
                                    .css("top", "0")
                                    .css("left", "0")
                                    .css("z-index", "10001");
        },
        _RemoveDeceive: function () {
            var formbox = this, options = this.Options;
            if (formbox._Deceive) {
                formbox._Deceive.remove();
                formbox._Deceive = null;
            }
        },
        _SetContent: function (content) {
            var formbox = this, options = this.Options;
            if (content && !options.Url) {
                formbox.Content.append($(content).show());
            }
        },
        _SetUrl: function (url) {
            var formbox = this, options = this.Options;
            if (url) {
                if (options.Params) {
                    url += url.indexOf('?') == -1 ? "?" : "&";
                    for (var name in options.Params) {
                        url += (name + "=" + options.Params[name] + "&");
                    }
                    url += ("random=" + new Date().getTime());
                }
                var iframe = $("iframe", formbox.Content);
                if (iframe.length > 0) {
                    iframe.attr("src", url);
                } else {
                    var framename = "Frame" + new Date().getTime();
                    iframe = $('<iframe onLoad="$.iFrameResize(this);"></iframe>').attr("id", framename).attr("name", framename)
                                                                               .attr("width", "100%").attr("frameborder", "0")
                                                                               .attr("scrolling", "no").attr("src", url)
                                                                               .appendTo(formbox.Content);
                }
            }
        },
        Show: function (pos) {
            var formbox = this, options = this.Options;
            formbox._SetCenter();
            if (pos)
                formbox.Set({ Top: pos.Top, Left: pos.Left });

            if (formbox.HasBind("BeforeShow")) {
                formbox.Trigger("BeforeShow", []);
            }

            formbox.Mask();
            $.Frame.WindowBase.SetFront(formbox);
            $(formbox.Element).show();

            if (formbox.HasBind("Shown")) {
                formbox.Trigger("Shown", []);
            }
        },
        Close: function () {
            var formbox = this, options = this.Options;

            if (formbox.HasBind("BeforeClose")) {
                formbox.Trigger("BeforeClose", []);
            }

            formbox.UnMask();
            $(formbox.Element).hide();

            if (formbox.HasBind("Close")) {
                formbox.Trigger("Close");
            }
        },
        Destroy: function () {
            var formbox = this, options = this.Options;

            formbox.Close();
            $(formbox.Element).remove();
            // 从组件管理器中移除
            $.Frame.Remove(formbox);
        },
        Load: function (url, params) {
            var formbox = this, options = this.Options;
            if (params)
                options.Params = $.extend(options.Params, params);

            formbox.Set({ Url: url });
        },
        Reload: function (params) {
            var formbox = this, options = this.Options;
            var url = options.Url;
            formbox.Load(url, params);
        },
        _SetCenter: function () {
            var formbox = this, options = this.Options;

            var windowHeight = window.outerHeight - window.screenTop;
            var windowWidth = $(window).width();
            //                        if (self.frameElement && self.frameElement.tagName == "IFRAME") {
            //                            windowHeight = $(parent.window).height();
            //                            windowWidth = $(parent.window).width();
            //                        }
            var panelHeight = $(formbox.Element).height();
            var panelWidth = $(formbox.Element).width();
            var left = 0, top = 0;

            var parentTarget = $(formbox.Element).parent();
            if (parentTarget.is("body")) {
                left = (windowWidth / 2) - (panelWidth / 2);
                top = (windowHeight / 2) + $(window).scrollTop() - (panelHeight / 2);
            }
            else {
                left = (parentTarget.width() / 2) - (panelWidth / 2);
                top = (parentTarget.height() / 2) - (panelHeight / 2);
            }

            formbox.Set({ Left: left, Top: top });
            formbox._SaveStatus();
        }
    });

    $.FormBox = function (options) {
        options = $.extend(options, { CloseOnDestroy: true });
        return $("<div></div>").FormBox(options);
    };

    $.FormBox.Show = function (options) {
        var form = $.FormBox(options);
        form.Show();
    };

})(jQuery);