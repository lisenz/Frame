/*
* FrameUI v2.1.0
*
* 作者：zlx
* 日期：2015.05.27
* 说明：CheckBox 复选框控件
*
* Copyright 2015 zlx
*
*/
(function ($) {
    $.fn.CheckBox = function (options) {
        return $.Frame.Do.call(this, "CheckBox", arguments);
    };

    $.Methods.CheckBox = $.Methods.CheckBox || {};

    $.Defaults.CheckBox = {
        Value: null,
        Checked: null,
        Disabled: false,
        OnChange: null
    };

    $.Frame.Controls.CheckBox = function (element, options) {
        $.Frame.Controls.CheckBox.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.CheckBox.Extension($.Frame.Controls.Base, {
        _GetType: function () {
            return "CheckBox";
        },
        _IdPrev: function () {
            return "CheckBoxFor";
        },
        _ExtendMethods: function () {
            return $.Methods.CheckBox;
        },
        _PreRender: function () {
            var checkbox = this, options = this.Options;

            // TODO：这里为了防止页面标签元素重复设置样式
            if (!$(checkbox.Element).hasClass("frame-checkbox")) {
                $(checkbox.Element).addClass("frame-checkbox");
            }

            options.Value = options.Value || $(checkbox.Element).val();
            if (options.Value == "on") {
                options.Value = "";
            }
        },
        _Render: function () {
            var checkbox = this, options = this.Options;

            var parent = $(checkbox.Element).parent();
            var wrapper = $("<div></div>").addClass("frame-checkbox-wrapper").appendTo(parent);
            var container = $("<label></label>").appendTo(wrapper);
            $(checkbox.Element).appendTo(container)
            checkbox.Label = $("<span></span>").addClass("lbl").appendTo(container);

            $(checkbox.Element).on("change", function (v) {
                if (checkbox.HasBind("Change"))
                    checkbox.Trigger("Change", [v.target.checked]);
                return false;
            });

            checkbox.Set(options);
        },
        _SetChecked: function (checked) {
            ///<summary>
            /// 设置复选框勾选状态
            ///</summary>
            ///<param name="checked" type="bool">设置一个值,该值标识是否复选框被勾选.</param>
            var checkbox = this, options = this.Options;
            $(checkbox.Element)[0].checked = checked;
        },
        _GetChecked: function () {
            ///<summary>
            /// 获取复选框勾选状态
            ///</summary>
            var checkbox = this, options = this.Options;
            return $(checkbox.Element)[0].checked
        },
        _SetValue: function (value) {
            ///<summary>
            /// 设置复选框显示文本
            ///</summary>
            ///<param name="value" type="string">复选框文本内容.</param>
            var checkbox = this, options = this.Options;
            if (value)
                checkbox.Label.text(value);
        },
        _GetValue: function () {
            ///<summary>
            /// 获取复选框显示文本
            ///</summary>
            var checkbox = this, options = this.Options;
            return checkbox.Label.text();
        },
        _SetDisabled: function (disabled) {
            ///<summary>
            /// 设置复选框禁用状态
            ///</summary>
            ///<param name="disabled" type="bool">设置一个值,该值标识复选框是否被禁用或启用[false:启用,true:禁用].</param>
            var checkbox = this, options = this.Options;
            $(checkbox.Element).attr("disabled", disabled);
        },
        _GetDisabled: function () {
            ///<summary>
            /// 获取复选框状态
            ///</summary>
            var checkbox = this, options = this.Options;
            return $(checkbox.Element).attr("disabled");
        },
        Checked: function (checked) {
            ///<summary>
            /// 设置复选框勾选状态
            ///</summary>
            ///<param name="checked" type="bool">复选框勾选状态.</param>
            var checkbox = this, options = this.Options;
            var disabled = $(checkbox.Element).attr("disabled");
            if (disabled)
                return;
            checkbox.Set({ Checked: checked });
            $(checkbox.Element).trigger("change");
        },
        Toggle: function () {
            ///<summary>
            /// 设置复选框勾选状态
            ///</summary>
            var checkbox = this, options = this.Options;
            var checked = $(checkbox.Element)[0].checked;
            checkbox.Checked(!checked);
        }
    });
})(jQuery);