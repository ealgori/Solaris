/*
    Mark Wiseman (http://wiseman.net.au)    
    jQuery MVC Template Row
    June 2012
    Use ASP.Net MVC3 strongly typed models to add new rows to tables client side
    License: MIT License
*/

(function ($) {
    $.MVCTemplateRow = function (element, options) {
        this.$el = $(element);
        this._init(options);
    }

    $.MVCTemplateRow.defaults = {
        templateTableName: 'mvcTemplateTable',  // the id of the hidden table to get the template row from
        templateFieldPrefix: 'Items',     // fields in the Template Table should be prefixed with this value
        newPk: 1,                               // this is used to track newly created rows and give them a unique id

        //events
        onAddRowAfter: function (tr) { return false; }     // fired after a row is added
    };

    $.MVCTemplateRow.prototype = {
        _init: function (options) {
            var $this = $(this),
                $table = this.$el;

            // Get settings from stored instance
            var settings = $this.data('settings');

            // Create or update the settings of the current mvcTemplateRow instance
            if (typeof (settings) == 'undefined') {
                settings = $.extend(true, {}, $.MVCTemplateRow.defaults, options);
            }
            else {
                settings = $.extend(true, {}, settings, options);
            }

            $this.data('settings', settings);
        },

        addRow: function () {
            var $this = $(this),
                settings = $this.data('settings'),
                $table = this.$el;

            $tableTemplate = $("#" + settings.templateTableName);

            //First check that we can find fields witht he correct anme format
            if ($tableTemplate.find('[id^=' + settings.templateFieldPrefix + ']').length == 0) {
                console.error('Cannot find any elements in #' + settings.templateTableName + ' that start with: ' + settings.templateFieldPrefix + '.');
                return;
            }

            settings.newPk -= 1;

            var newRowIndex = $table.find('tbody > tr').length;

            //clone the template row and place it in the table
            $tableTemplate.find("tr").attr("pk", settings.newPk);
            $tableTemplate.find("tr").clone().appendTo($table.find("tbody:last"));
            $tableTemplate.find("tr").attr("pk", "null"); //reset this so there aren't 2 out there with the same value

            //get the new tr
            var tr = $table.find("tr[pk=" + settings.newPk + "]");

            //rename the inputs so MVC can pick them up

            tr.find("input").each(function () {
                resetTemplateElements($(this), newRowIndex, settings.templateFieldPrefix);
            });

            tr.find("select").each(function () {
                resetTemplateElements($(this), newRowIndex, settings.templateFieldPrefix);
            });

            //rename the validation spans
            tr.find(".field-validation-valid").each(function () {
                $(this).attr("data-valmsg-for", $(this).attr("data-valmsg-for").replace("0", newRowIndex));
//                $(this).attr("data-valmsg-for", $(this).attr("data-valmsg-for").replace(settings.templateFieldPrefix, ""));
            });

            //re-apply validation
            $('form').removeData("validator");
            $('form').removeData("unobtrusiveValidation");
            $.validator.unobtrusive.parse('form');
            settings.onAddRowAfter(tr);
        },

        deleteRow: function (tr) {
            var $this = $(this),
                settings = $this.data('settings'),
                $table = this.$el;

            //remove the row
            tr.remove();
            //reindex the inputs so MVC can pick them up
            settings.newPk = 1;
            $table.find("tbody > tr").each(function (f) {
                settings.newPk -= 1;
                this.attributes.pk.nodeValue = settings.newPk;
                
            });

            var index = 0;
            $table.find("tbody > tr").each(function () {
                $(this).find("input").each(function () {
                    resetTemplateElements($(this), index, settings.templateFieldPrefix);
                });

                $(this).find("select").each(function () {
                    resetTemplateElements($(this), index, settings.templateFieldPrefix);
                });

                index++;
            });

        }
    }

    $.fn.mvcTemplateRow = function (options) {
        args = Array.prototype.slice.call(arguments, 1);

        this.each(function () {

            var instance = $(this).data('mvcTemplateRow');

            if (typeof (options) === 'string') {
                if (!instance) {
                    console.error('Methods cannot be called until jquery.mvcTemplateRow has been initialized.');
                }
                else if (!$.isFunction(instance[options])) {
                    console.error('The method "' + options + '" does not exist in jquery.mvcTemplateRow.');
                }
                else {
                    instance[options].apply(instance, args);
                }
            }
            else {
                if (!instance) {
                    $(this).data('mvcTemplateRow', new $.MVCTemplateRow(this, options));
                }
            }

        });

        return this;
    };

    function resetTemplateElements(element, newId, templateFieldPrefix) {
        var elementId = element.attr("id");

        if (typeof (elementId) != 'undefined') {
            //            element.attr("id", element.attr("id").replace(templateFieldPrefix, ""));
            //            element.attr("name", element.attr("name").replace(templateFieldPrefix, ""));

            element.attr("id", element.attr("id").replace(/_(\d+)__/, "_" + newId + "__"));
            element.attr("name", element.attr("name").replace(/\[(\d+)\]/, "[" + newId + "]"));
        }

        element.removeClass('hasDatepicker');
    }
})(jQuery);



