/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.22
* 说明：Wizard 向导控件。
*/
(function ($) {
    $.fn.Wizard = function (options) {
        return $.Frame.Do.call(this, "Wizard", arguments);
    };

    $.Methods.Wizard = $.Methods.Wizard || {};

    $.Defaults.Wizard = {
        Steps: [],       //{Step,StepTarget,Title}
        WizardTarget: null,
        ActionTarget: null,
        OnActive: null,
        OnComplete: null
    };

    $.Frame.Controls.Wizard = function (element, options) {
        $.Frame.Controls.Wizard.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Wizard.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Wizard";
        },
        _IdPrev: function () {
            return "WizardFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Wizard;
        },
        _PreRender: function () {
            var wizard = this;

            if (!$(wizard.Element).hasClass("frame-wizard")) {
                $(wizard.Element).addClass("frame-wizard");
            }
            wizard.Step = -1;
        },
        _Render: function () {
            var wizard = this, options = this.Options;

            wizard.Container = $("<ul></ul>").addClass("frame-wizard-steps").appendTo(wizard.Element);
            $(wizard.Element).attr("wizard-target", options.WizardTarget).attr("action-target", options.ActionTarget);

            wizard.Set(options);
        },
        _SetSteps: function (steps) {
            var wizard = this, options = this.Options;
            if (steps && steps.length > 0) {
                $(steps).each(function () {
                    var step = $("<li></li>").attr("step", this.Step).attr("step-target", this.StepTarget)
                                             .appendTo(wizard.Container);
                    step.append($("<span></span>").addClass("step").text(this.Step));
                    step.append($("<span></span>").addClass("title").text(this.Title));
                    $(this.StepTarget).addClass("frame-wizard-step-pane");
                });
                wizard.Next();
            }
        },
        _SetWizardTarget: function (target) {
            var wizard = this, options = this.Options;
            if (target) {
                $(wizard.Element).attr("wizard-target", target);
                $(target).addClass("frame-wizard-step-content");
            }
        },
        _SetActionTarget: function (target) {
            var wizard = this, options = this.Options;
            if (target) {
                $(wizard.Element).attr("action-target", target);
                $(target).addClass("frame-wizard-actions");
            }
        },
        Previous: function () {
            var wizard = this, options = this.Options;

            if (wizard.Step > 0) {
                wizard.Step--;

                var current = $(wizard.Container).children().eq(wizard.Step);
                var next = current.next();
                if (next && next.length > 0) {
                    next.removeClass("complete").removeClass("active");
                    $(next.attr("step-target")).removeClass("active");
                }
                current.removeClass("complete").addClass("active");
                $(current.attr("step-target")).addClass("active");
            }
        },
        Next: function () {
            var wizard = this, options = this.Options;

            if (wizard.Step < options.Steps.length - 1) {
                wizard.Step++;

                var current = $(wizard.Container).children().eq(wizard.Step);
                var prev = current.prev();
                current.addClass("active");
                $(current.attr("step-target")).addClass("active");
                if (prev && prev.length > 0) {
                    prev.addClass("complete");
                    $(prev.attr("step-target")).removeClass("active");
                    if (wizard.HasBind("Complete"))
                        wizard.Trigger("Complete", [prev, { Step: parseInt(prev.attr("step")), Title: $("span.title", prev).text()}]);
                }
                if (wizard.HasBind("Active"))
                    wizard.Trigger("Active", [current, { Step: parseInt(current.attr("step")), Title: $("span.title", current).text()}]);
            }
        }
    });

})(jQuery);