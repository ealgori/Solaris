app.module("Route", function (Route, App, Backbone, Marionette) {
    Route.Controller = Marionette.Controller.extend({
        initialize: function () {
            console.log("controller init");
            this.router = new Route.Router({ controller: this });
            var avrCollection = new App.Entities.AvrCollection();

            var avrView = new App.Views.AvrView({ collection: avrCollection });



            avrCollection.fetch({
                success: function () {
                    avrView.render();
                }
            });

           // var detailModel = new App.Entities.DetailModel();
          //  var detailView = new App.Views.DetialView({ model: detailModel });


          //  var itemModel = new App.Entities.ItemCollection();
          //  var itemsView = new App.Views.ItemsView({ collection: itemModel });
            
         //   App.detailsRegion.show(detailView);
         //   itemsView.render();
           // App.itemsRegion.show(itemsView);


            //detailView.$el.hide();
            //detailView.$el.fadeIn(400, function () { console.log("anim complete") });

          //  detailView.render();

        },
        index: function () {
            var data = { "Status": "error", "Message": "Пожалуйста выберите ТО" };
            var detailModel = new App.Entities.DetailModel(data);
            var detailPanelView = new App.Views.DetailPanelView({ model: detailModel });
            app.detailsRegion.show(detailPanelView);
        },
        showTo: function (to) {


            //var detailModel = new App.Entities.DetailModel();
            //App.detailsRegion.currentView.model = detailModel;
            var model = new App.Entities.DetailModel();// App.detailsRegion.currentView.model;
            model.url = model.baseUrl + $.param({ TOId: to });
            App.currentTO = to;
            //var opt;
          //  Backbone.fetchCache.clearItem(model);
            
            model.fetch({
                //data: $.param({ TOId: to }),
                //prefill: true,
               // prefillExpires: 200,
               // prefillSuccess: function(data){console.log("from cache"); console.log(data);}, // Fires when the cache hit happens
               // success: function(data){console.log("from ajax"); console.log(data);}, // Fires after the AJAX call
               

                beforeSend: function () {
                    console.log("loading details");
                },
                //cache: false,
                success: function (data) {

                    if (data.get("Message")) {
                        var detailPanelView = new App.Views.DetailPanelView({ model: data });
                        app.detailsRegion.show(detailPanelView);
                       
                    }
                    else {
                        var detailView = new App.Views.DetialView({ model: data });
                        app.detailsRegion.show(detailView);
                        var itemCollection = new App.Entities.ItemCollection(data.get("Items"));
                        //itemCollection.reset(data.get('Items'));
                        var itemsView = new App.Views.ItemsView({ collection: itemCollection });
                        itemsView.render();



                    }



                    //if (data.get("Items")) {
                    //    var itemCollection = new App.Entities.ItemCollection(data.get("Items"));
                    //    //itemCollection.reset(data.get('Items'));
                    //    var itemsView = new App.Views.ItemsView({ collection: itemCollection });
                    //    itemsView.render();
                    //    //App.Prices = new app.Entities.PriceItemCollection();
                    //    //App.Prices.fetch({
                    //    //    data: { TOId: to },
                    //    //    success: function (data) {
                    //    //        //// console.log("fetch completed");
                    //    //        //for (index = 0; index < itemCollection.length; ++index) {
                    //    //        //    var model = itemCollection.at(index);
                    //    //        //    var price = App.Prices.findWhere({ value: parseInt(model.get("ItemId")) });
                    //    //        //    if (price)
                    //    //        //        model.set("TOItemText", price.get("text"));
                    //    //        //}

                    //    //    }
                    //    //});
                    //}


                    //  }
                    //  else
                    //  {
                    //  $("#items").html("");
                    //  }
                },

            }).done(function (data) {
                //App.detailsRegion.currentView.render();
                //console.log(data);
            });


            console.log("to" + to);
        },



    }


    );

    Route.Router = Marionette.AppRouter.extend({
        appRoutes: {
            "": "index",
            "to/:to": "showTo"
        },
        // controller: new Route.Controller()

    })

})