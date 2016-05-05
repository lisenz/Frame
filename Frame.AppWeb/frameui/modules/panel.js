/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.10
* 说明：Panel 面板控件。
*/
(function ($) {
    $.fn.Panel = function (options) {
        return $.Frame.Do.call(this, "Panel", arguments);
    };

    $.Methods.Panel = $.Methods.Panel || {};

    $.Defaults.Panel = {
        HasHeader: false,
        Title: "无标题",
        Icon: "icon-desktop",
        ToolBar: [],      // {Icon,OnClick}
        HasFooter: false,
        Commands: [],    // Button
        Width: null,
        Height: null,
        Plain: false,    // 简洁效果，即不带边框
        Url: null,
        Params: {},
        Content: null
    };

    $.Frame.Controls.Panel = function (element, options) {
        $.Frame.Controls.Panel.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Panel.Extension($.Frame.Controls.WindowBase, {
        _GetType: function () {
            return "Panel";
        },
        _IdPrev: function () {
            return "PanelFor";
        },
        _ExtenMethods: function () {
            return $.Methods.Panel;
        },
        _PreRender: function () {
            var panel = this, options = this.Options;
            if (!$(panel.Element).hasClass("frame-panel")) {
                $(panel.Element).addClass("frame-panel");
            }

            if ($(panel.Element).children().length > 0) {
                options.Content = $(panel.Element).children().first().detach();
                $(panel.Element).empty();
            }
        },
        _Render: function () {
            var panel = this, options = this.Options;

            panel.Header = $("<div></div>").addClass("frame-panel-header");
            panel.Container = $("<div></div>").addClass("frame-panel-container");
            panel.Inner = $("<div></div>").addClass("frame-panel-inner").appendTo(panel.Container);
            panel.Content = $("<div></div>").addClass("frame-panel-main").appendTo(panel.Inner);
            panel.Footer = $("<div></div>").addClass("frame-panel-toolbox").addClass("clearfix")
                                           .append("<hr />").appendTo(panel.Inner);

            $(panel.Element).append(panel.Header).append(panel.Container);

            panel.TimerId = null;
            panel.Set(options);
        },
        _SetHasHeader: function (hasHeader) {
            var panel = this, options = this.Options;
            if (!hasHeader) {
                panel.Header.remove();
            }
        },
        _SetTitle: function (title) {
            var panel = this, options = this.Options;
            if (title && options.HasHeader) {
                $("<h5></h5>").addClass("lighter").append($("<span></span>").text(title)).appendTo(panel.Header);
            }
        },
        _SetIcon: function (icon) {
            var panel = this, options = this.Options;
            if (options.HasHeader) {
                $("h5", panel.Header).prepend($("<i></i>").addClass(icon));
            }
        },
        _SetToolBar: function (tools) {
            var panel = this, options = this.Options;
            if (options.HasHeader && tools && tools.length > 0) {
                var bar = $("<div></div>").addClass("frame-panel-toolbar").appendTo(panel.Header);
                $(tools).each(function () {
                    var tool = this;
                    var btn = $("<a></a>").append($("<i></i>").addClass(tool.Icon)).appendTo(bar);
                    if (tool.OnClick) {
                        btn.on("click", function () {
                            tool.OnClick.call(this, panel);
                        });
                    }
                });
            }
        },
        _SetHasFooter: function (hasFooter) {
            var panel = this, options = this.Options;
            if (!hasFooter) {
                panel.Footer.remove();
            }
        },
        _SetCommands: function (commands) {
            var panel = this, options = this.Options;
            if (options.HasFooter && commands && commands.length > 0) {
                $(commands).each(function () {
                    var button = $("<button></button>").Button(this);
                    var orientation = this.Orientation == "right" ? "pull-right" : "pull-left";
                    panel.Footer.append($(button.Element).addClass(orientation));
                });
            }
        },
        _SetContent: function (content) {
            var panel = this, options = this.Options;
            if (content) {
                panel.Content.append($(content));
            }
        },
        _SetUrl: function (url) {
            var panel = this, options = this.Options;
            if (url) {
                if (options.Params) {
                    url += url.indexOf('?') == -1 ? "?" : "&";
                    for (var name in options.Params) {
                        url += (name + "=" + options.Params[name] + "&");
                    }
                    url += ("random=" + new Date().getTime());
                }
                var iframe = $("iframe", panel.Content);
                if (iframe.length > 0) {
                    iframe.attr("src", url);
                } else {
                    var framename = "Frame" + new Date().getTime();
                    iframe = $('<iframe onLoad="$.iFrameResize(this);"></iframe>').attr("id", framename).attr("name", framename)
                                                                               .attr("width", "100%").attr("frameborder", "0")
                                                                               .attr("scrolling", "no").attr("src", url)
                                                                               .appendTo(panel.Content);

                    // TODO：这里使用定时循环监控iframe中的页面高度，当高度有变时重置iframe高度
                    if (panel.TimerId)
                        window.clearInterval(panel.TimerId);
                    panel.TimerId = setInterval(function () { $.iFrameResize(iframe[0]); }, 250);
                }
            }
        },
        _SetWidth: function (width) {
            var panel = this, options = this.Options;
            if (width)
                $(panel.Element).width(width);
        },
        _SetHeight: function (height) {
            var panel = this, options = this.Options;
            if (height) {
                var head = options.HasHeader ? panel.Header.outerHeight(true) : 0;
                var foot = options.HasFooter ? panel.Footer.outerHeight(true) : 0;
                height = height - head - foot - 24;
                panel.Content.height(height);
            }
        },
        _SetPlain: function (plain) {
            var panel = this, options = this.Options;
            if (plain) {
                $(panel.Element).addClass("no-border");
            }
        },
        Load: function (url, params) {
            var panel = this, options = this.Options;
            if (params)
                options.Params = $.extend(options.Params, params);

            panel.Set({ Url: url });
        },
        Reload: function (params) {
            var panel = this, options = this.Options;
            var url = options.Url;
            panel.Load(url, params);
        }
    });


})(jQuery);
