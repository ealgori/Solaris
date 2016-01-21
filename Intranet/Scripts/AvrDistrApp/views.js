app.module("Views", function (Views, App, Backbone, Marionette, $, _) {

    Views.ItemView = Marionette.ItemView.extend({
        template: "#recipient-template",
        tagName: "tr",
        events: {
            "click .toggle": "toggleEnabled",
            "click .destroy": "del",
            "click td": "edit"


        },
        edit: function () {
            var dialogView = new app.Views.DialogView({ model: this.model });
            app.dialog.show(dialogView);
            $('#dialog').modal();

        },
        del: function (ev) {
            ev.stopPropagation();
            App.trigger("recipient:delete", this.model);
        },
        toggleEnabled: function (ev) {
            ev.stopPropagation();
            var enabled = this.model.get("Enabled");
            //this.model.set("Enabled", !enabled);
            var self = this;
            this.model.save({ "Enabled": !enabled }, {
                wait: true,
                error: function () {
                    //self.model.attributes = self.model._previousAttributes;
                    self.model.trigger("change");
                }
            });

        },
        modelEvents:
            { "change": "render" }


    });
    Views.ListView = Marionette.CompositeView.extend({
        ///  el:"#app",
        template: "#recipients-template",
        childView: Views.ItemView,
        childViewContainer: "tbody",


        filter: function (child, index, collection) {
            if (!this.options.filterText)
                return true;

            var result = false;
            if (child.get('Name'))
                result = result || (child.get('Name').toLowerCase().indexOf(this.options.filterText) > -1);
            if (child.get('RukOtdelaEmail'))
                result = result || (child.get('RukOtdelaEmail').toLowerCase().indexOf(this.options.filterText) > -1);
            if (child.get('RukFillialaEmail'))
                result = result || (child.get('RukFillialaEmail').toLowerCase().indexOf(this.options.filterText) > -1);
            if (child.get('POROREmail'))
                result = result || (child.get('POROREmail').toLowerCase().indexOf(this.options.filterText) > -1);
            return result;


        }



    });


    Views.DialogView = Marionette.ItemView.extend({
        template: '#modal_content_with_close_btn',
        //initialize: function () {
        //    if (this.model)
        //        Backbone.Validation.bind(this);
        //},
        events: {
            'click #save': function (e) {
                e.preventDefault();
                var serializes = Backbone.Syphon.serialize(this);

                if (!this.model)
                    App.trigger("recipient:add", serializes);
                else
                    this.model.save(serializes, { wait: true, error:function(){alert("Извините, произошла ошибка сохранения.")} });

            },
            'keyup input':"preValidate"
        },
        modelEvents: {
            "invalid": "showInvalid"
        },
        showInvalid: function (ev, errors) {
            for (var error in errors) {

                $("[name='" + error + "Error" + "']").html(errors[error]);
                // propertyName is what you want
                // you can get the value like this: myObject[propertyName]
            }
            //this.model.validationError.forEach(function (err) {
            //    console.log(err);
            //});

        },
        preValidate:function(){
            this.model.trigger("validate");
        }
    });

    Views.Header = Marionette.ItemView.extend({
        template: "#header-template",
        ui: { search: ".search-query" },
        events:
            {
                "click #create-new": "showCreate",
                "keyup @ui.search": "filterList"
            },
        showCreate: function () {
            var dialogView = new app.Views.DialogView();
            app.dialog.show(dialogView);
            $('#dialog').modal();
        },
        filterList: function (ev) {
            var searchText = $(this.ui.search).val();
            App.trigger("setFilter", searchText);

        }
    });

})