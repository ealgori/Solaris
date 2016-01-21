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