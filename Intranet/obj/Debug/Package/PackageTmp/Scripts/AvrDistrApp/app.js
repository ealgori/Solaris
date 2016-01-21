
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
