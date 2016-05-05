/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.05
* 说明：SideBar 侧边栏控件。
*/
(function ($) {
    $.fn.SideBar = function (options) {
        return $.Frame.Do.call(this, "SideBar", arguments);
    };

    $.Methods.SideBar = $.Methods.SideBar || {};

    $.Defaults.SideBar = {
        Shortcuts: null,   // 快捷键 [{Icon,Mode,OnClick}]
        Content: null       // 侧边栏中包裹的内容,这里可以一个function函数返回jquery标签对象或者字符串表示的标签
    };

    $.Frame.Controls.SideBar = function (element, options) {
        $.Frame.Controls.SideBar.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.SideBar.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "SideBar";
        },
        _IdPrev: function () {
            return "SideBarFor";
        },
        _ExtendMethods: function () {
            return $.Methods.SideBar;
        },
        _PreRender: function () {
            var sidebar = this, options = this.Options;

            if (!$(sidebar.Element).hasClass("frame-sidebar")) {
                $(sidebar.Element).addClass("frame-sidebar");
            }

            if ($(sidebar.Element).children().length > 0) {
                options.Content = $(sidebar.Element).children().first().detach();
                $(sidebar.Element).empty();
            }
        },
        _Render: function () {
            var sidebar = this, options = this.Options;
            sidebar.Set(options);

            sidebar.Collapse = $("<i></i>").addClass("icon-double-angle-left")
            .on("click", function () {
                if ($(this).hasClass("icon-double-angle-left")) {
                    $(sidebar.Element).addClass("min-sidebar");
                    $(this).removeClass("icon-double-angle-left").addClass("icon-double-angle-right");
                } else {
                    $(sidebar.Element).removeClass("min-sidebar");
                    $(this).removeClass("icon-double-angle-right").addClass("icon-double-angle-left");
                }
                return false;
            });
            $("<div></div>").addClass("frame-sidebar-collapse").append(sidebar.Collapse).appendTo(sidebar.Element);
        },
        _SetShortcuts: function (shortcuts) {
            var sidebar = this, options = this.Options;

            if (shortcuts && shortcuts.length > 0) {
                sidebar.ShortcutsContainer = $("<div></div>").addClass("frame-sidebar-shortcuts").appendTo(sidebar.Element);
                sidebar._CreateLargeShortcuts(shortcuts);
                sidebar._CreateMiniShortcuts(shortcuts);
            }
        },
        _CreateLargeShortcuts: function (shortcuts) {
            var sidebar = this, options = this.Options;
            var container = $("<div></div>").addClass("frame-sidebar-shortcuts-large").appendTo(sidebar.ShortcutsContainer);
            $(shortcuts).each(function () {
                var button = $("<button></button>").addClass("frame-button").on("click", this.OnClick);
                var icon = $("<i></i>").addClass(this.Icon).appendTo(button);
                this.Mode && button.addClass("frame-button-" + this.Mode);
                this.Title && $(button).Tip({ Placement: "auto", Mode: "inverse", Title: this.Title });
                button.appendTo(container);
            });
        },
        _CreateMiniShortcuts: function (shortcuts) {
            var sidebar = this, options = this.Options;
            var container = $("<div></div>").addClass("frame-sidebar-shortcuts-mini").appendTo(sidebar.ShortcutsContainer);
            $(shortcuts).each(function () {
                var button = $("<span></span>").addClass("frame-button");
                this.Mode && button.addClass("frame-button-" + this.Mode);
                button.appendTo(container);
            });
        },
        _SetContent: function (content) {
            var sidebar = this, options = this.Options;

            if (content) {
                var detail = ("function" == typeof content ? content.call(sidebar.Element) : content);
                $(sidebar.Element).append($(content));
            }
        }
    });



})(jQuery);