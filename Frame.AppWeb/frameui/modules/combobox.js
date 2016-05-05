/**
* jQuery FrameUI 2.1.0
* 
* 作者：zlx
* 日期：2015.06.20
* 说明：Combobox 控件。
*/
(function ($) {
    $.fn.ComboBox = function (options) {
        return $.Frame.Do.call(this, "ComboBox", arguments);
    };

    $.Methods.ComboBox = $.Methods.ComboBox || {};

    $.Defaults.ComboBox = {
        Editable: true,
        Width: 100,
        PanelWidth: null,
        PanelHeight: null,
        Mode: "primary",
        //Resize: true,           //下拉列表面板是否允许调整大小
        Value: "Key",
        Text: "Value",
        //ApplyToolTip: false,    // 该属性用于当下拉框选项文本超出长度以省略号隐藏时，鼠标移动到下拉框上方时以提示信息显示全文
        DefaultText: "请选择",
        Datas: null,            // [{ Key,Value }]
        Url: null,              //数据源URL(需返回JSON)
        Method: "POST",
        DataType: "json",
        Params: null,
        Disabled: false,
        OnBeforeSelect: false, //选择前事件
        OnSelected: null, //选择值事件 
        OnSuccess: null,
        OnError: null,
        OnComplete: null,
        OnResolve: null,
        OnBeforeOpen: null      //打开下拉框前事件，可以通过return false来阻止继续操作，利用这个参数可以用来调用其他函数，比如打开一个新窗口来选择值
    };

    $.Frame.Controls.ComboBox = function (element, options) {
        $.Frame.Controls.ComboBox.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.ComboBox.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "ComboBox";
        },
        _IdPrev: function () {
            return "ComboBoxFor";
        },
        _ExtendMethods: function () {
            return $.Methods.ComboBox;
        },
        _PreRender: function () {
            var combobox = this, options = this.Options;
            if (!$(combobox.Element).hasClass("frame-combobox")) {
                $(combobox.Element).addClass("frame-combobox");
            }

            // 这里Selection变量主要用来记录当前下拉框所选选项信息
            combobox.Selection = { Text: options.DefaultText, Value: "-1" };
        },
        _Render: function () {
            var combobox = this, options = this.Options;

            combobox.Container = $("<div></div>").addClass("frame-combobox-container")
                                    .on("click", function () { combobox.ToggleDropDown(); });
            combobox.Inner = $("<div></div>").addClass("frame-combobox-inner");
            combobox.SelectionPanel = $("<div></div>").addClass("frame-combobox-selection-panel");
            combobox.Menu = $("<ul></ul>").addClass("frame-combobox-menu").appendTo(combobox.SelectionPanel);

            $(combobox.Element).wrap(combobox.Inner);
            $(combobox.Element).parent().wrap(combobox.Container);
            $(combobox.Element).parent().after(combobox.SelectionPanel);

            combobox.Set(options);
        },
        _SetEditable: function (editalbe) {
            var combobox = this, options = this.Options;
            if (editalbe) {
                var input = $('<input type="text" />').addClass("frame-combobox-text");
                $(combobox.Element).parent().append(input);
            } else {
                var div = $("<div></div>").addClass("frame-combobox-text");
                $(combobox.Element).parent().append(div);
            }

            var angle = $("<i></i>").addClass("icon-sort-down");
            $(combobox.Element).parent().append(angle);
        },
        _SetWidth: function (width) {
            var combobox = this, options = this.Options;
            if (width) {
                var angleWidth = $("i.icon-sort-down", $(combobox.Element).parent()).outerWidth(true);
                if (options.Editable)
                    $("input.frame-combobox-text", $(combobox.Element).parent()).width(width - angleWidth - 10);
                else
                    $("div.frame-combobox-text", $(combobox.Element).parent()).width(width - angleWidth - 10);
            }
        },
        _SetPanelWidth: function (width) {
            var combobox = this, options = this.Options;
            if (width) {
                combobox.Menu.width(width);
            }
            else {
                combobox.Menu.width($(combobox.Element).parent().width());
            }
        },
        _SetPanelHeight: function (height) {
            var combobox = this, options = this.Options;
            if (height) {
                combobox.SelectionPanel.height(height);
            }
        },
        _SetMode: function (mode) {
            var combobox = this, options = this.Options;
            if (mode)
                combobox.Menu.addClass("frame-combobox-" + mode);
        },
        _SetDisabled: function (disabled) {
            var combobox = this, options = this.Options;

            if (disabled) {
                $(combobox.Element).parent().addClass("disabled");
                if (options.Editable)
                    $("input.frame-combobox-text", $(combobox.Element).parent()).attr("disabled", true);
            } else {
                $(combobox.Element).parent().removeClass("disabled");
                if (options.Editable)
                    $("input.frame-combobox-text", $(combobox.Element).parent()).removeAttr("disabled");

            }
        },
        _SetDatas: function (datas) {
            var combobox = this, options = this.Options;

            var index = 0;
            if (options.DefaultText) {
                combobox.Menu.append(combobox._CreateListItem(index, -1, options.DefaultText));
            }

            if ($(combobox.Element).children().length > 0) {
                $(combobox.Element).children("option").each(function (idx) {
                    index = index + 1;
                    combobox.Menu.append(combobox._CreateListItem(index, this.value, this.text));
                });
            }

            if (datas) {
                if (combobox.HasBind("Resolve"))
                    datas = combobox.Trigger("Resolve", [datas]);
                $(datas).each(function (idx) {
                    index = index + 1;
                    combobox.Menu.append(combobox._CreateListItem(index, this[options.Value], this[options.Text]));
                });
            }

            combobox.Select(0);
        },
        _CreateListItem: function (idx, value, text) {
            var combobox = this;
            var item = $("<li></li>").attr("idx", idx).on("click", function () {
                var index = parseInt($(this).attr("idx"));
                combobox.Select(index);
                return false;
            });
            var link = $("<a></a>").attr("href", "#").attr("val", value).text(text).appendTo(item);
            return item;
        },
        _SetUrl: function (url) {
            var combobox = this, options = this.Options;
            if (url != null) {
                combobox.Menu.empty();
                $.ajax({
                    dataType: options.DataType, type: options.Method,
                    url: url,
                    data: options.Params,
                    success: function (data) {
                        if (combobox.HasBind("Success"))
                            data = combobox.Trigger("Success", [data]);
                        combobox.Set({ Datas: data });
                    },
                    error: function (xhr, info, exp) {
                        // 三个参数:XMLHttpRequest对象、错误信息、(可选)捕获的异常对象
                        if (combobox.HasBind("Error"))
                            combobox.Trigger("Error", [{ XHR: xhr, Info: info, Exp: exp}]);
                    },
                    complete: function (xhr, ts) {
                        if (combobox.HasBind("Complete"))
                            combobox.Trigger("Complete", [{ XHR: xhr, TS: ts}]);
                        combobox.Select(0);
                    }
                });
            }
        },
        Disable: function () {
            // 禁用控件
            var combobox = this, options = this.Options;
            combobox.Set({ Disabled: true });
        },
        Enable: function () {
            // 启用控件
            var combobox = this, options = this.Options;
            combobox.Set({ Disabled: false });
        },
        Select: function (which) {
            ///<summary>
            /// 选择指定的选项
            ///</summary>
            ///<param name="which" type="which">选项的索引值或名称.当参数为索引值时,起始值从0开始.</param>
            var combobox = this, options = this.Options;

            var pass = true;
            if (combobox.HasBind("BeforeSelect"))
                pass = combobox.Trigger("BeforeSelect", []);

            if (pass) {
                var listitem = null;
                var item = null;
                if (typeof which == "number") {
                    $("li.active", combobox.Menu).removeClass("active");
                    listitem = combobox.Menu.children().eq(which).addClass("active");
                    item = $("a", listitem);
                    combobox.Selection = { Text: item.text(), Value: item.attr("val") };
                } else {
                    for (var i = 0, count = combobox.Menu.children().length; i < count; i++) {
                        listitem = combobox.Menu.children().eq(i);
                        item = $("a", listitem);
                        if (item.text() == which) {
                            $("li.active", combobox.Menu).removeClass("active");
                            listitem.addClass("active");
                            combobox.Selection = { Text: item.text(), Value: item.attr("val") };
                        }
                    }
                }

                if (options.Editable)
                    $("input.frame-combobox-text", $(combobox.Element).parent()).val(combobox.Selection.Text);
                else
                    $("div.frame-combobox-text", $(combobox.Element).parent()).text(combobox.Selection.Text);

                if (combobox.HasBind("Selected"))
                    combobox.Trigger("Selected", [listitem, combobox.Selection]);
            }

            combobox.SelectionPanel.slideUp("fast");
            return combobox.Selection;
        },
        ToggleDropDown: function () {
            var combobox = this, options = this.Options;
            var canOpen = true;
            if (!combobox.SelectionPanel.is(":visible") && combobox.HasBind("BeforeOpen"))
                canOpen = combobox.Trigger("BeforeOpen", []);

            if (!$(combobox.Element).parent().hasClass("disabled") && canOpen)
                combobox.SelectionPanel.slideToggle("fast");
        },
        GetText: function () {
            ///<summary>
            /// 获取当前选中的选项文本
            ///</summary>
            var combobox = this, options = this.Options;
            return combobox.Selection.Text;
        },
        SetText: function (text) {
            ///<summary>
            /// 设置当前选项文本
            ///</summary>
            ///<param name="text" type="string">要选中的选项文本</param>
            var combobox = this, options = this.Options;
            combobox.Select(text);
        },
        GetValue: function () {
            ///<summary>
            /// 获取当前选中的选项值
            ///</summary>
            var combobox = this, options = this.Options;
            return combobox.Selection.Value;
        },
        SetValue: function (value) {
            ///<summary>
            /// 设置当前选项值
            ///</summary>
            ///<param name="value" type="string">要选中的选项值</param>
            var combobox = this, options = this.Options;
            for (var i = 0, count = combobox.Menu.children().length; i < count; i++) {
                var listitem = combobox.Menu.children().eq(i);
                var item = $("a", listitem);
                if (item.attr("val") == value) {
                    $("li.active", combobox.Menu).removeClass("active");
                    listitem.addClass("active");
                    combobox.Selection = { Text: item.text(), Value: item.attr("val") };
                }
            }

            if (options.Editable)
                $("input.frame-combobox-text", $(combobox.Element).parent()).val(combobox.Selection.Text);
            else
                $("div.frame-combobox-text", $(combobox.Element).parent()).text(combobox.Selection.Text);
        },
        Clear: function () {
            ///<summary>
            /// 重置下拉框
            ///</summary>
            var combobox = this, options = this.Options;
            combobox.Selection = { Text: options.DefaultText, Value: "-1" };
            combobox.Select(0);
        },
        Reload: function (params) {
            ///<summary>
            /// 重新进行远程请求数据并绑定到下拉框
            ///</summary>
            ///<param name="params" type="object">请求远程数据参数</param>
            var combobox = this, options = this.Options;
            options.Params = params;
            combobox.Set({ Url: options.Url });
        },
        LoadData: function (datas) {
            ///<summary>
            /// 加载数据并绑定到下拉框
            ///</summary>
            ///<param name="datas" type="array">要进行加载绑定的数据集合</param>
            var combobox = this, options = this.Options;
            combobox.Menu.empty();
            combobox.Set({ Datas: datas });
            combobox.Select(0);
        }
    });
})(jQuery);