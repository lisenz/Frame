/**
* jQuery FrameUI v2.1.0
* 
* 作者：zlx
* 日期：2015.07.01
* 说明：Tags 标签控件。
*/
(function ($) {
    $.fn.Tags = function (options) {
        return $.Frame.Do.call(this, "Tags", arguments);
    };

    $.Methods.Tags = $.Methods.Tags || {};

    $.Defaults.Tags = {
        Items: [],    // {Text,Closable}
        Plain: true,  // 是否为简洁模式,即无边框
        Width: null,
        OnItemClick: null
    };

    $.Frame.Controls.Tags = function (element, options) {
        $.Frame.Controls.Tags.Parent.constructor.call(this, element, options);
    };

    $.Frame.Controls.Tags.Extension($.Frame.UIComponent, {
        _GetType: function () {
            return "Tags";
        },
        _IdPrev: function () {
            return "TagsFor";
        },
        _ExtendMethods: function () {
            return $.Methods.Tags;
        },
        _PreRender: function () {
            var tags = this;

            if (!$(tags.Element).hasClass("frame-tags")) {
                $(tags.Element).addClass("frame-tags");
            }

            tags.Children = [];
        },
        _Render: function () {
            var tags = this, options = this.Options;

            $(tags.Element).empty();
            tags.Set(options);
        },
        _SetPlain: function (plain) {
            var tags = this;
            if (!plain)
                $(tags.Element).addClass("frame-tags-bordered");
        },
        _SetWidth: function (width) {
            var tags = this;
            if (width)
                $(tags.Element).width(width);
        },
        _SetItems: function (items) {
            var tags = this, options = this.Options;
            if (items && items.length > 0) {
                $(items).each(function (i) {
                    var item = this;
                    tags.Children.push(item);
                    var tag = $("<li></li>").text(item.Text).on("click", function () {
                        $(".frame-tags-selected", tags.Element).removeClass("frame-tags-selected");
                        $(this).addClass("frame-tags-selected");
                        if (tags.HasBind("ItemClick"))
                            tags.Trigger("ItemClick", [this, $(tags.Element).children().index($(this)[0])]);
                    });
                    tag.appendTo(tags.Element);
                    if (item.Closable) {
                        tag.addClass("with-x");
                        $("<i>×</i>").appendTo(tag).on("click", function () {
                            var idx = $(tags.Element).children().index($(this).parent()[0]);
                            tags.RemoveTag(idx);
                        });
                    }
                });
            }
        },
        RemoveTag: function (which) {
            var tags = this, options = this.Options;

            if (!tags.Exists(which))
                return;

            var index = (typeof which == "number") ? which : tags.GetTagIndex(which);
            $(tags.Element).children().eq(index).remove();
            tags.Children.splice(index, 1);
            options.Items.splice(index, 1);
        },
        AppendTag: function (item) {
            var tags = this, options = this.Options;

            if (tags.Exists(item.Text))
                return;

            options.Items.push(item);
            tags._SetItems([item]);
        },
        AppendRangeTag: function (items) {
            var tags = this, options = this.Options;
            $(items).each(function () {
                tags.AppendTag(this);
            });
        },
        Exists: function (which) {
            ///<summary>
            /// 返回一个值，该值标识是否已存在指定索引或名称的标签
            ///</summary>
            ///<param name="which" type="which">标签的索引值或名称</param>
            var tags = this, options = this.Options;
            var exists = false;

            if (tags.Children.length == 0)
                return false;

            if (typeof which == "number") {
                if (which < 0 || tags.Children.length <= which)
                    return false;
                return true;
            }
            if (typeof which == "string") {
                $(tags.Children).each(function () {
                    if (this.Text == which) {
                        exists = true;
                        return;
                    }
                });

                return exists;
            }

            return false;
        },
        GetTagIndex: function (text) {
            ///<summary>
            /// 获取指定名称的标签的对应索引值
            ///</summary>
            ///<param name="text" type="string">标签的名称</param>
            var tags = this, options = this.Options;

            var index = -1;
            $(tags.Children).each(function (idx) {
                if (this.Text == text) {
                    index = idx;
                    return;
                }
            });

            return index;
        }
    });

})(jQuery);