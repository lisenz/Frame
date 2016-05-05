/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.06.20
* 说明：Radio 单选框控件。
*/
(function ($) {
    $.fn.Radio = function (options) {
        return $.Frame.Do.call(this, "Radio", arguments);
    };

    $.Methods.Radio = $.Methods.Radio || {};

    $.Defaults.Radio = {
        Value: null,
        Checked: null,
        Disabled: false,
        OnChange: null
    };

    $.Frame.Controls.Radio = function (element, options) {
        $.Frame.Controls.Radio.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Radio.Extension($.Frame.Controls.Base, {
        _GetType: function () {
            return "Radio";
        },
        _IdPrev: function () {
            return "RadioFor";
        },
        _ExtenMethods: function () {
            return $.Methods.Radio;
        },
        _PreRender: function () {
            var radio = this, options = this.Options;

            if (!$(radio.Element).hasClass("frame-radio")) {
                $(radio.Element).addClass("frame-radio");
            }

            options.Value = options.Value || $(radio.Element).val();
            options.Checked = options.Checked || $(radio.Element)[0].checked;
        },
        _Render: function () {
            var radio = this, options = this.Options;

            var parent = $(radio.Element).parent();

            radio.Container = $(radio.Element).wrap("<label></label>").parent();
            radio.Label = $("<span></span>").addClass("lbl").insertAfter(radio.Element);
            radio.Wrapper = $(radio.Element).parent().wrap($("<div></div>").addClass("frame-checkbox-wrapper")).parent();

            $(radio.Element).on("change", function (v) {
                if (radio.HasBind("Change"))
                    radio.Trigger("Change", [v.target.checked]);
                return false;
            });

            radio.Set(options);
        },
        _SetValue: function (value) {
            var radio = this, options = this.Options;
            if (value)
                radio.Label.text(value);
        },
        _GetValue: function () {
            var radio = this, options = this.Options;
            return radio.Label.text();
        },
        _SetDisabled: function (disabled) {
            ///<summary>
            /// 设置单选框禁用状态
            ///</summary>
            ///<param name="disabled" type="bool">设置一个值,该值标识复选框是否被禁用或启用[false:启用,true:禁用].</param>
            var radio = this, options = this.Options;
            if (disabled)
                $(radio.Element).attr("disabled", disabled);
            else
                $(radio.Element).removeAttr("disabled");
        },
        _GetDisabled: function () {
            ///<summary>
            /// 获取单选框状态
            ///</summary>
            var radio = this, options = this.Options;
            return $(radio.Element).attr("disabled");
        },
        Status: function () {
            var radio = this, options = this.Options;
            return radio._GetChecked();
        },
        _SetChecked: function (checked) {
            ///<summary>
            /// 设置单选框勾选状态
            ///</summary>
            ///<param name="checked" type="bool">设置一个值,该值标识是否复选框被勾选.</param>
            var radio = this, options = this.Options;
            $(radio.Element)[0].checked = checked;
        },
        _GetChecked: function () {
            ///<summary>
            /// 获取单选框勾选状态
            ///</summary>
            var radio = this, options = this.Options;
            return $(radio.Element)[0].checked
        },
        Checked: function (checked) {
            var radio = this, options = this.Options;
            var disabled = $(radio.Element).attr("disabled");
            if (disabled)
                return;
            radio.Set({ Checked: checked });
            $(radio.Element).trigger("change");
        }
    });

})(jQuery);