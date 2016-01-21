app.module("Entities", function (Entities, App, Backbone, Marionette) {
    Entities.AvrModel = Backbone.Model.extend({
        defaults: {
            text: "",
            value:"0"
        }
    });
    Entities.AvrCollection = Backbone.Collection.extend({
        url: "/Json/TOList"
    });

    Entities.DetailModel = Backbone.Model.extend({
       
        baseUrl:"/TO/GetTODataByIdGet?"

    });

    Entities.ItemModel = Backbone.Model.extend({
        defaults: {
            Site: 0,
            SiteAddress: 0,
            Type: 'не указан',
            SiteQuantity: 0,
            TOItem: 0,
            Price: 0,
            Total: 0,
            selected:false
        },
        toggle: function () {
            var selected = this.get("selected");
            this.set("selected",!selected);
        }
    });

    Entities.ItemCollection = Backbone.Collection.extend({
        //fetch: function (options) {
        //    options = options || {};
        //    options.cache = false;
        //    return Backbone.Collection.prototype.fetch.call(this, options);
        //}
    });

    Entities.PriceItem = Backbone.Model.extend({
        defaults: {
            text: "no text",
            value: "no value"
        },

    });
    Entities.PriceItemCollection = Backbone.Collection.extend({
        url: "/Json/TOPositionListGet"
    })


})