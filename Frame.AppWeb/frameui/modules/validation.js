/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.12.14
* 说明：Validation 表单校验控件。
*/
(function ($) {
    $.fn.Validation = function (options) {
        return $.Frame.Do.call(this, "Validation", arguments);
    };
    $.fn.ValidateInput = function (options) {
        return $.Frame.Do.call(this, "ValidateInput", arguments);
    };

    $.Methods.Validation = $.Methods.Validation || {};
    $.Methods.ValidateInput = $.Methods.ValidateInput || {};

    $.Defaults.Validation = {
        OnError: null,
        OnSuccess: null
    };
    $.Defaults.ValidateInput = {
        OnError: null,
        OnSuccess: null
    };

    $.Frame.Controls.Validation = function (element, options) {
        $.Frame.Controls.Validation.Parent.constructor.call(this, element, options);
    };
    $.Frame.Controls.ValidateInput = function (element, options) {
        $.Frame.Controls.Validation.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Validation.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Validation";
        },
        _IdPrev: function () {
            return "ValidationFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Validation;
        },
        _PreRender: function () {
            var form = this;

            if (!$(form.Element).hasClass("frame-validation")) {
                $(form.Element).addClass("frame-validation");
            }
        },
        _Render: function () {
            var form = this, options = this.Options;

            form.Results = [];
            form.Inputs = [];
            form.Counter = 0;
            form._Init();

            form.Set(options);
        },
        _Init: function () {
            var form = this, options = this.Options;

            form.Results = [];
            form.Inputs.splice(0, form.Inputs.length);
            form.Counter = 0;

            $(form.Element).find("input:visible").each(function (index, input) {
                //排除 hidden、button、submit、checkbox、radio、file
                if (input.type != "hidden" && input.type != "button" && input.type != "submit" && input.type != "checkbox" && input.type != "radio" && input.type != "file") {
                    var checker = $(input).ValidateInput({
                        OnError: function (e) {
                            form.Results.push(e);
                            if (form.HasBind("Error")) {
                               return form.Trigger("Error", [e]);
                            }
                            return true;
                        },
                        OnSuccess: function (e) {
                            form.Counter++;
                            if (form.Counter == form.Inputs.length) {
                                form.Counter = 0;
                                if (form.HasBind("Success")) {
                                  return  form.Trigger("Success", [e]);
                                }
                            }
                            return true;
                        }
                    });

                    form.Inputs.push(checker);
                }
            });
        },
        Validation: function () {
            var form = this, options = this.Options;
            form._Init();

            var index;
            for (index in form.Inputs) {
                form.Inputs[index].Validation();
            }
        }
    });

    $.Frame.Controls.ValidateInput.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "ValidateInput";
        },
        _IdPrev: function () {
            return "ValidateInputFor";
        },
        _ExtendMethods: function () {
            return $.Methods.ValidateInput;
        },
        _PreRender: function () {
            var input = this;

            input.Rules = [];
            input.Counter = 0;
            input.Message = $(input.Element).attr("message");
            input.Message = (!!input.Message ? input.Message : "格式错误!");

        },
        _Render: function () {
            var input = this, options = this.Options;

            input._Init_Popover();
            input.Set(options);

            //是否实时检查
            var ruleString = $(input.Element).attr("rules");
            if (!!ruleString && -1 != ruleString.indexOf("real-time")) {
                $(input.Element).blur(function () {
                    input.Validation();
                });
            }

        },
        _Init_Popover: function () {
            var input = this;

            input.Popover = $(input.Element).Popover({ Mode: "notitle", Placement: "left auto" });
        },
        Validation: function () {
            var input = this;

            input.Value = $(input.Element).val();
            input.Counter = 0;
            input.Rules = [];

            var ruleString = $(input.Element).attr("rules");
            input._Parse(ruleString);

            for (var i = 0; i < input.Rules.length; i++) {
                //调用条件函数
                if (!!input.Judges[input.Rules[i].Rule])
                    input.Judges[input.Rules[i].Rule](input, input.Value, input.Rules[i].Param);
            }
        },
        Judges: {
            "char-number": function (input, value, param) {
                if (value != "" && !(/^[0-9]*$/g.test(value)))
                    return input._Error("char-number");
                else

                    return input._Success("char-number");
            },
            "char-normal": function (input, value, param) {
                if (false == /^\w+$/.test(value))
                    return input._Error("char-normal");
                else
                    return input._Success("char-normal");
            },
            "char-chinese": function (input, value, param) {
                if (false == /^([\w]|[\u4e00-\u9fa5]|[ 。，、？￥“‘！：【】《》（）——+-])+$/.test(value))
                    return input._Error("char-chinese");
                else
                    return input._Success("char-chinese");
            },
            "char-english": function (input, value, param) {
                if (false == /^([\w]|[ .,?!$'":+-])+$/.test(value))
                    return input._Error("char-english");
                else
                    return input._Success("char-english");
            },
            "email": function (input, value, param) {
                if (false == /^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$/.test(value))
                    return input._Error("email");
                else
                    return input._Success("email");
            },
            "length": function (input, value, param) {
                var range = param.split("-");

                //如果长度设置为 length:6 这样的格式
                if (range.length == 1) range[1] = range[0];

                if (value.length < range[0] || value.length > range[1])
                    return input._Error("length");
                else
                    return input._Success("length");
            },
            "equal": function (input, value, param) {
                var pair = $(param);
                if (0 == pair.length || pair.val() != value)
                    return input._Error("equal");
                else
                    return input._Success("equal");
            },
            "ajax": function (input, value, param) {
                if (false == eval(param))
                    return input._Error("ajax");
                else
                    return input._Success("ajax");
            },
            "date": function (input, value, param) {
                if (false == /^(\d{4})-(\d{2})-(\d{2})$/.test(value))
                    return input._Error("date");
                else
                    return input._Success("date");
            },
            "time": function (input, value, param) {
                if (false == /^(\d{2}):(\d{2}):(\d{2})$/.test(value))
                    return input._Error("time");
                else
                    return input._Success("time");
            },
            "datetime": function (input, value, param) {
                if (false == /^(\d{4})-(\d{2})-(\d{2}) (\d{2}):(\d{2}):(\d{2})$/.test(value))
                    return input._Error("datetime");
                else
                    return input._Success("datetime");
            },
            "money": function (input, value, param) {
                if (false == /^([1-9][\d]{0,7}|0)(\.[\d]{1,2})?$/.test(value))
                    return input._Error("money");
                else
                    return input._Success("money");
            },
            "uint": function (input, value, param) {
                value = parseInt(value);
                param = parseInt(param);

                if (isNaN(value) || isNaN(param) || value < param || value < 0)
                    return input._Error("uint");
                else
                    return input._Success("uint");
            },
            "require": function (input, value, param) {
                if (!value) {
                    if (!!param && -1 != param.indexOf("require")) //rule不为空并且含有require
                        return input._Error("require");
                    else
                        return input._Success("require");

                }
            }
        },
        _Parse: function (ruleString) {
            var input = this;

            input.Rules = [];

            var rules = !!ruleString ? ruleString.split(";") : {};

            for (var i = 0; i < rules.length; i++) {
                var s = rules[i];
                var rule = s;
                var param = "";

                //有：号
                var p = s.indexOf(":");
                if (-1 != p) {
                    rule = s.substr(0, p);
                    param = s.substr(p + 1);
                }

                if (!!input.Judges[rule])
                    input.Rules.push({ Rule: rule, Param: param });
            }
        },
        _Error: function (rule) {
            var input = this, options = this.Options;
            var isContinue = true;
            if (input.HasBind("Error")) {
                isContinue = input.Trigger("Error", [rule]);
            }

            if (isContinue) {
                var msg = $(input.Element).attr(rule + "-message");

                var msg = !msg ? this.Message : msg;

                input.Popover.Reset(msg);
                input.Popover.Show();
                return true;
            }

            return false;
        },
        _Success: function (rule) {
            var input = this, options = this.Options;

            input.Counter += 1;

            if (input.Counter == input.Rules.length) {
                var isContinue = true;
                if (input.HasBind("Success")) {
                    isContinue = input.Trigger("Success", [rule]);
                }

                if (isContinue) {
                    input.Popover.Close();
                }
            }

            return true;
        }
    });

})(jQuery);