Backbone.Marionette.TemplateCache.prototype.compileTemplate = function (rawTemplate, options) {
    // use Handlebars.js to compile the template
    return Handlebars.compile(rawTemplate);
}

var App = Marionette.Application.extend({
    
})


window.app = new App();
app.addRegions({
    ddMenu: ".ddmenu",
    detailsRegion: "#details",
    itemsRegion: "#items-list",
    matItemsRegion:"#matitems-list"
});
app.on('start', function () {
    var controller = new app.Route.Controller();
    app.Prices = new app.Entities.PriceItemCollection();
    app.TestModel = new app.Entities.DetailModel();
    app.TestModel.url = app.TestModel.baseUrl;
   
    //var ddView = new app.Views.PricesView({ collection: app.Prices });
    //app.ddMenu.show(ddView);
    // alert('started');
})

app.on("before:start", function () {
   
    //var router = new App.Route.Router({ controller: controller });
})

$(function () {
    
    app.start();
    Backbone.history.start();
})
