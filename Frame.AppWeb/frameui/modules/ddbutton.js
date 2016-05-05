/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.15
* 说明：DropDownButton 下拉按钮控件。
*/
(function ($) {
    $.fn.DropDownButton = function (options) {
        return $.Frame.Do.call(this, "DropDownButton", arguments);
    };

    $.Methods.DropDownButton = $.Methods.DropDownButton || {};

    $.Defaults.DropDownButton = {
        Text: "",
        Size: null,
        Mode: "primary",
        MenuItems: [],   //{Text,Divider}
        Disabled: false,
        OnItemClick: null
    };

    $.Frame.Controls.DropDownButton = function (element, options) {
        $.Frame.Controls.DropDownButton.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.DropDownButton.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "DropDownButton";
        },
        _IdPrev: function () {
            return "DropDownButtonFor";
        },
        _ExtendMethods: function () {
            return $.Methods.DropDownButton;
        },
        _PreRender: function () {
            var ddbutton = this;

            if (!$(ddbutton.Element).hasClass("frame-button-group")) {
                $(ddbutton.Element).addClass("frame-button-group");
            }
        },
        _Render: function () {
            var ddbutton = this, options = this.Options;

            ddbutton.Btn = $("<button></button>").addClass("frame-button").appendTo(ddbutton.Element);
            ddbutton.Menu = $("<ul></ul>").addClass("frame-dropdown-menu").addClass("frame-dropdown-" + options.Mode);
            $(ddbutton.Element).append(ddbutton.Menu);

            ddbutton.Btn = ddbutton.Btn.Button($.extend(options, { Icon: "icon-caret-down", IconSite: "right", OnClick: function () {
                if ($(ddbutton.Menu).children().length > 0) {
                    ddbutton.Menu.width($(ddbutton.Element).width());
                    $(ddbutton.Element).toggleClass("open");
                }
            }
            }));

            ddbutton.Set(options);
        },
        _SetMenuItems: function (items) {
            var ddbutton = this, options = this.Options;
            if (items && items.length > 0) {
                $(items).each(function (idx) {
                    if (this.Divider) {
                        $("<li></li>").addClass("divider").appendTo(ddbutton.Menu);
                    }
                    else if (this.Text) {
                        var abtn = $("<a></a>").attr("href", "#").attr("idx", idx).text(this.Text).on("click", function () {
                            if (ddbutton.HasBind("ItemClick"))
                                ddbutton.Trigger("ItemClick", [$(this).text(), $(this).attr("idx")]);
                            $(ddbutton.Element).toggleClass("open");
                            return false;
                        });
                        var li = $("<li></li>").appendTo(ddbutton.Menu);
                        li.append(abtn);
                    }
                });
            }
        },
        Disable: function () {
            var ddbutton = this, options = this.Options;
            ddbutton.Btn.Disable();
        },
        Enable: function () {
            var ddbutton = this, options = this.Options;
            ddbutton.Btn.Enable();
        },
        _GetDisabled: function () {
            var ddbutton = this, options = this.Options;
            return ddbutton.Btn.Disabled;
        }
    });

})(jQuery);