/**
* jQuery FrameUI 2.1.0
* 
* 作者：zlx
* 日期：2015.06.22
* 说明：Spinner 控件。
*/
(function ($) {
    $.fn.Spinner = function (options) {
        return $.Frame.Do.call(this, "Spinner", arguments);
    };

    $.Methods.Spinner = $.Methods.Spinner || {};

    $.Defaults.Spinner = {
        Type: "float",     // int,float,time
        Value: null,
        Width: 100,
        ReadOnly: null,
        Disabled: null,
        Step: 0.1,         // 每次递增或递减的步长值
        IsNegative: false, // 是否只能为负数
        Place: 2,          // 小数保留位数,当Type=float时生效
        MinValue: 0,
        MaxValue: 100,
        OnChange: null
    };

    $.Frame.Controls.Spinner = function (element, options) {
        $.Frame.Controls.Spinner.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Spinner.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Spinner";
        },
        _IdPrev: function () {
            return "SpinnerFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Spinner;
        },
        _PreRender: function () {
            var spinner = this, options = this.Options;

            if (!$(spinner.Element).hasClass("frame-spinner")) {
                $(spinner.Element).addClass("frame-spinner");
            }

            spinner.Disabled = false;
            spinner.oldValue = null;
        },
        _Render: function () {
            var spinner = this, options = this.Options;

            spinner.Container = $("<div></div>").addClass("frame-spinner-container");
            spinner.Group = $("<div></div>").addClass("frame-spinner-group");
            $(spinner.Element).wrap(spinner.Group);
            $(spinner.Element).parent().wrap(spinner.Container);

            spinner.DeButtonGroup = $("<div></div>").addClass("frame-spinner-buttons").insertBefore(spinner.Element)
                                                    .on("click", function () { spinner._DoDecrease(); });
            spinner.DeButton = $("<button></button>").addClass("frame-button").addClass("frame-button-error")
                                                     .append($("<i></i>").addClass("icon-minus").addClass("smaller-75"))
                                                     .appendTo(spinner.DeButtonGroup);

            spinner.InButtonGroup = $("<div></div>").addClass("frame-spinner-buttons").insertAfter(spinner.Element)
                                                    .on("click", function () { spinner._DoIncrease(); });
            spinner.InButton = $("<button></button>").addClass("frame-button").addClass("frame-button-success")
                                                     .append($("<i></i>").addClass("icon-plus").addClass("smaller-75"))
                                                     .appendTo(spinner.InButtonGroup);

            $(spinner.Element).on("change", function () {
                var value = $(this).val();
                var newValue = spinner._GetVerifyValue(value);
                if (spinner.HasBind("Change") && spinner.oldValue != newValue)
                    spinner.Trigger("Change", [newValue]);
                $(this).val(newValue);
                return false;
            });
            spinner.Set(options);
        },
        _SetWidth: function (width) {
            var spinner = this, options = this.Options;
            var btnWidth = spinner.DeButtonGroup.outerWidth(true);

            $(spinner.Element).parent().parent().width(width);
            $(spinner.Element).width(width - btnWidth * 2 - 10);
        },
        _SetValue: function (value) {
            var spinner = this, options = this.Options;
            if (value) {
                value = spinner._GetVerifyValue(value);
                $(spinner.Element).val(value);
                $(spinner.Element).trigger("change");
            } else {
                $(spinner.Element).val(spinner._GetDefaultValue());
            }
        },
        _GetValue: function () {
            var spinner = this;
            return $(spinner.Element).val();
        },
        _SetDisabled: function (disabled) {
            var spinner = this;
            if (disabled) {
                $(spinner.Element).attr("disabled", "disabled");
                spinner.DeButton.attr("disabled", "disabled");
                spinner.DeButton.addClass("disabled");
                spinner.InButton.attr("disabled", "disabled");
                spinner.InButton.addClass("disabled");
                spinner.Disabled = true;
            } else {
                $(spinner.Element).removeAttr("disabled");
                spinner.DeButton.removeAttr("disabled");
                spinner.DeButton.removeClass("disabled");
                spinner.InButton.removeAttr("disabled");
                spinner.InButton.removeClass("disabled");
                spinner.Disabled = false;
            }
        },
        _GetDisabled: function () {
            var spinner = this;
            return spinner.Disabled;
        },
        _SetReadOnly: function (readonly) {
            var spinner = this;
            if (readonly != null) {
                if (readonly)
                    $(spinner.Element).attr("readonly", "readonly");
                else
                    $(spinner.Element).removeAttr("readonly");
            }
        },
        _GetReadOnly: function () {
            var spinner = this;
            return $(spinner.Element).attr("readonly");
        },
        _IsInt: function (value) {
            ///<summary>
            /// 判断数值是否为整数
            ///</summary>
            var spinner = this, options = this.Options;
            var rule = options.IsNegative ? /^-\d+$/ : /^\d+$/;

            if (!rule.test(value))
                return false;
            if (parseInt(value) != value)
                return false;

            return true;
        },
        _IsFloat: function (value) {
            ///<summary>
            /// 判断数值是否为浮点数
            ///</summary>
            var spinner = this, options = this.Options;
            var rule = options.IsNegative ? /^-\d+(\.\d+)?$/ : /^\d+(\.\d+)?$/;

            if (!rule.test(value))
                return false;
            if (parseFloat(value) != value)
                return false;

            return true;
        },
        _IsTime: function (value) {
            ///<summary>
            /// 判断是否为时间
            ///</summary>
            var spinner = this, options = this.Options;
            var time = value.match(/^(\d{1,2}):(\d{1,2})$/);

            if (time == null)
                return false;
            if (time[1] >= 24 || time[2] >= 60)
                return false;
            return true;
        },
        _GetVerifyValue: function (value) {
            ///<summary>
            /// 获取经过格式验证处理后的数值
            ///</summary>
            ///<param name="value" type="number">要进行格式验证处理的数值</param>
            var spinner = this, options = this.Options;
            var newValue = null;
            switch (options.Type) {
                case "float":
                    newValue = spinner._ToRound(value, options.Place);
                    break;
                case "time":
                    newValue = value;
                    break;
                default:
                    newValue = parseInt(value);
                    break;
            }
            if (!spinner._Verify(newValue))
                return spinner._GetDefaultValue();
            else
                return newValue;
        },
        _Verify: function (value) {
            ///<summary>
            /// 验证数值是否符合指定类型格式且不超过数值范围
            ///</summary>
            ///<param name="value" type="number">验证的数值</param>
            var spinner = this, options = this.Options;
            var result = true;
            var val;
            switch (options.Type) {
                case "float":
                    if (!spinner._IsFloat(value))
                        result = false;
                    else {
                        val = parseFloat(value);
                        result = spinner._VerifyNumberRange(val);
                    }
                    break;
                case "time":
                    result = spinner._IsTime(value);
                    break;
                default:
                    if (!spinner._IsInt(value))
                        result = false;
                    else {
                        val = parseInt(value);
                        result = spinner._VerifyNumberRange(val);
                    }
                    break;
            }

            return result;
        },
        _VerifyNumberRange: function (value) {
            ///<summary>
            /// 验证数值是否超过限制范围
            ///</summary>
            ///<param name="value" type="number">验证的数值</param>
            var spinner = this, options = this.Options;

            if (options.MinValue != undefined && options.MinValue > value)
                return false;
            if (options.MaxValue != undefined && options.MaxValue < value)
                return false;
            return true;
        },
        _GetDefaultValue: function () {
            ///<summary>
            /// 根据配置类型返回文本框显示的默认值
            ///</summary>
            var spinner = this, options = this.Options;
            var val = null;
            switch (options.Type) {
                case "time":
                    val = "00:00";
                    break;
                case "float":
                    val = spinner._ToRound(0, options.Place);
                    break;
                default:
                    val = 0;
                    break;
            }

            return val;
        },
        _ToRound: function (value, place) {
            ///<summary>
            /// 将指定数值转换为保留指定位数
            ///</summary>
            ///<param name="value" type="number">要保留指定小数位的数值</param>
            ///<param name="place" type="number">数值要保留的小数位位数</param>
            var spinner = this, options = this.Options;
            return parseFloat(value).toFixed(place);
        },
        _CalcValue: function (step) {
            var spinner = this, options = this.Options;
            var value = $(spinner.Element).val();
            spinner.Set({ Value: parseFloat(value) + step });
        },
        _CalcTime: function (step) {
            var spinner = this, options = this.Options;
            var value = $(spinner.Element).val();
            var matchValue = value.match(/^(\d{1,2}):(\d{1,2})$/);
            var newMinute = parseInt(matchValue[2]) + step;
            var newHour = parseInt(matchValue[1]);

            if (newMinute < 10 && newMinute >= 0)
                newMinute = "0" + newMinute;
            if (newMinute == 60) {
                newHour += step;
                newMinute = "00";
            }
            if (newHour == 24) {
                newHour = "00";
            }
            if (newMinute < 0) {
                newHour += step;
                newMinute = 59;
            }
            if (newHour < 10)
                newHour = "0" + newHour;

            value = newHour + ":" + newMinute;
            spinner.Set({ Value: value });
        },
        _DoIncrease: function () {
            ///<summary>
            /// 向上递增
            ///</summary>
            var spinner = this, options = this.Options;

            if (options.Disabled || options.ReadOnly)
                return;

            switch (options.Type) {
                case "time":
                    spinner._CalcTime(options.Step);
                    break;
                default:
                    spinner._CalcValue(options.Step);
                    break;
            }
        },
        _DoDecrease: function () {
            ///<summary>
            /// 向下递减
            ///</summary>
            var spinner = this, options = this.Options;

            if (spinner.Disabled || options.ReadOnly)
                return;

            switch (options.Type) {
                case "time":
                    spinner._CalcTime(options.Step * -1);
                    break;
                default:
                    spinner._CalcValue(options.Step * -1);
                    break;
            }
        },
        Enable: function () {
            var spinner = this, options = this.Options;
            spinner.Set({ Disabled: false });
        },
        Disable: function () {
            var spinner = this, options = this.Options;
            spinner.Set({ Disabled: true });
        },
        SetValue: function (value) {
            var spinner = this;
            spinner.Set({ Value: value });
        },
        GetValue: function () {
            var spinner = this;
            return $(spinner.Element).val();
        }
    });

})(jQuery);