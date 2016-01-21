///#source 1 1 /Scripts/AvrDistrApp/app.js

'use strict';
var app = new Marionette.Application();
Marionette.TemplateCache.prototype.compileTemplate = function (rawTemplate, options) {
    // use Handlebars.js to compile the template
    return Handlebars.compile(rawTemplate);
};
app.addRegions({
    "header":"#header",
    "main": "#app",
    "dialog":"#dialog"
})

app.on("start", function () {
    console.log("Hello world");
    var headerView = new app.Views.Header();
    app.header.show(headerView);
    Backbone.history.start();
    app.recipients = new app.Models.Recipients();
    app.recipients.fetch(
        {
        //success: function (data) {
        //    var view = new app.Views.ListView({ collection: data });
        //    app.main.show(view);
            //}
            error: function () {
                alert("There is an error while getting recipients collection");
            }
        }
    ).done(function () {
        var view = new app.Views.ListView({ collection: app.recipients });
        app.main.show(view);
    });

   
    
   
    app.on("recipient:add", function (attributes) {
        app.recipients.create(attributes, {wait:true});
    });
    app.on("recipient:delete", function (model) {
        if(model)
            console.log(model.destroy({ wait: true }));
    })

    app.on("setFilter", function (queryText) {
        var view = new app.Views.ListView({ collection: app.recipients, filterText: queryText.toLowerCase() });
        app.main.show(view);
    })

   

});
$(function () {

   
    app.start();
})

///#source 1 1 /Scripts/AvrDistrApp/models.js
app.module("Models", function (Models, App, Backbone, Marionette, $, _) {
    Models.Recipient = Backbone.Model.extend({
        //validation: {
        //    Name: {
        //        required: true,

        //        msg: 'Subregion name is required'
        //    },
        //    RukOtdelaEmail: {
        //        pattern: 'email',
        //       // msg: 'Пожалуйста укажите коректные адреса почты'
        //    },
        //    RukOtdelaEmail: {
        //        pattern: 'email',
        //       // msg: 'Пожалуйста укажите коректные адреса почты'
        //    },
        //    RukOtdelaEmail: {
        //        pattern: 'email',
        //       // msg: 'Пожалуйста укажите коректные адреса почты'
        //    }

        //},
        //validate: function(args){
        //    var errors = [];
        //    if (args.Name.trim() == "") {
        //        errors.push({ 'message': "please enter name", 'field': "Name" });
        //    }
        //    if (errors.length)
        //        return errors;
        //    else
        //        return false;
        //},
        idAttribute: "Id",
        methodToURL: function (method, id) {


            if (method == "create")
                return "post"
            if (method == "update")
                return "put";
            if (method == "delete")
                return "delete/" + id;
            return "get";



        },
        sync: function (method, model, options) {
            options = options || {};
            options.url = this.methodToURL(method.toLowerCase(), this.id);
            return Backbone.sync(method, model, options);
        }

        // url: function () { return "get"; }

    });

    Models.Recipients = Backbone.Collection.extend({
        model: Models.Recipient,
        url: "get"
        ,
        comparator: function (chapter) {
            return chapter.get("Name");
        }
    });
})
///#source 1 1 /Scripts/AvrDistrApp/route.js

///#source 1 1 /Scripts/AvrDistrApp/views.js
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
