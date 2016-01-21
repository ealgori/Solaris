app.module("Views", function (Views, App, Backbone, Marionette) {

    Views.AvrView = Marionette.ItemView.extend({
        // model: App.Entities.AvrModel,
        template: "#to-combo-template",
        el: "#to",
        onRender: function () {
            this.$el.find('#to-select').combobox();
        },
        events:{
        "change #to-select": "navigate"
        },
        navigate: function (ev) {
            var select = ev.currentTarget;
            var value = $(select).val();
            Backbone.history.navigate(value, { trigger: true });

        }
    })

    //Views.AvrComboBox = Marionette.CompositeView.extend({
    //    template: "#avr-combo-template",
    //    el: "#avr",
    //    childView: Views.AvrView,
    //    childViewContainer:"#avr-opt"
    //})

   

    Views.DetailPanelView = Marionette.ItemView.extend({
        template: "#detail-panel-template",
    });

    Views.DetialView = Marionette.ItemView.extend({
        template: "#detail-template",
        // el: "#details",
        modelEvents: {
            "all": "render"
        },
        //serializeData: function () {
           
        //    if(!this.model.get("TotalPrice"))
        //        this.model.set("TotalPrice", "не указано");
        //    if (!this.model.get("POIssueDate"))
        //        this.model.set("POIssueDate", "не указано");

        //    return {
        //        TotalPrice: this.model.get("TotalPrice"),
        //        POIssueDate: this.model.get("POIssueDate"),
        //        Activity: this.model.get("POIssueDate"),
        //        Brunch: this.model.get("Brunch"),
        //        Region: this.model.get("Region"),
        //        Subcontractor: this.model.get("Subcontractor"),
        //        Type: this.model.get("Type"),
        //        WOVAT: this.model.get("WOVAT"),

               
        //    };


        //},

        onRender: function () {
            //if (this.model.get("Items")) {
            //    //var itemCollection = new App.Entities.ItemCollection(this.model.get("Items"));
            //    ////itemCollection.reset(data.get('Items'));
            //    //var itemsView = new App.Views.ItemsView({ collection: itemCollection });
            //    //itemsView.render();
            //    App.Prices = new app.Entities.PriceItemCollection();
            //    App.Prices.fetch();
            //    //App.Prices.fetch({
            //    //    data: { TOId: App.currentTO },
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
        }

    });


    Views.ItemView = Marionette.ItemView.extend({
        template: "#row-template",
        events: {
            "click .view": "editing",
            "click #check": "check",
            // "change .edit": "finishEditing",
            "keyup .edit": "cancellEditing"
        },
        check:function(ev){
            ev.preventDefault();
            //var obj = ev.currentTarget;
            var selected = this.model.get("selected")
            if (selected===undefined)
                selected = false;
            var site = this.model.get("Site");
            this.model.collection.each(function (item) {
                var itemSite = item.get("Site");
                if (site == itemSite)
                {
                    item.set("selected", !selected);
                }
            });

            
            //$(obj).prop("checked", selected);
            //this.model.toggle;

            
          //  console.log(ev);
        },
        editing: function (ev) {
           // var dropDown = new Views.PricesView({ collection: App.Prices });
          //  var price = App.Prices.findWhere({ value: this.model.get("ItemId") });
            //var cv = dropDown.children.findByModel(price);


           // dropDown.$el.find("[value=" + price.get('value') + "]").attr("selected", true);
           // dropDown.render();
          //  dropDown.$el.find('select').combobox();
            //this.$el.find('.edit').html(dropDown.el);
           // this.listenTo(App, "price:selected", function (val) { this.setPrice(val); });
           // this.listenTo(App, "price:cancell", function (val) { this.cancellEdit(); });
            this.$el.addClass('editing');;
        },
        setPrice: function (priceId) {

            var price = App.Prices.findWhere({ value: parseInt(priceId) });
            this.model.set("TOItemText", price.get("text"));
            this.model.set("ItemId", price.get("value"));
            console.log(price);
            this.$el.removeClass('editing');;
        },
        cancellEdit: function () {
            this.$el.removeClass('editing');;
        },

        finishEditing: function (ev) {
            var val = $(ev.currentTarget).find('select').val();
            var model = app.Prices.findWhere({ '': val });
            this.model.set("TOItem", val);
            this.$el.removeClass('editing');;
        },
        cancelEditing: function (e) {
            console.log(e);
            this.$el.children('.select').toggleClass("editing");
        },
        modelEvents: {
            "all": "render"
        },
        onRender: function () {

        },
        tagName: "tr",

        //templateHelpers: function () {
        //    //return {
        //        TOItem: this.model.get('TOItem').toJSON()
        //    }
        //}

    })
    Views.ItemsView = Marionette.CollectionView.extend({
        //template: "#items-template",
        el: "#items-list",
        childView: Views.ItemView,
        prefill: true,
        prefillSuccess: function () {
            console.log("prefill:"+App.TestModel.get("Brunch"));
        },
        //ui: {
        //    "mod1": "#fetch-model1",
        //    "mod2": "#fetch-model2",
        //    "mod3": "#fetch-model3"
        //},
        //events: {
        //    "click @ui.mod1": "fetch1",
        //    "click @ui.mod2": "fetch2",
        //    "click @ui.mod3": "fetch3"
        //},
        //fetch1: function (ev) {

        //    this.fetch("КРАСНОЯРСК,ТО ПСКВ-1");
              

        //},
        //fetch2: function (ev) {
        //    this.fetch("СУРГУТ_ТО_АУГПТ_7");
        //},
        //fetch3: function (ev) {
        //    this.fetch("SIB ABN ТО1 АМС");
        //},

        //fetch: function (to) {
        //    App.TestModel.fetch({
        //        data: $.param({ TOId: to}),
        //        success: function (data) {
        //            console.log("success:");
        //            console.log(data.attributes.Brunch);
        //            console.log(App.TestModel.get("Brunch"));
        //        },
        //        cache: false
        //    }).done(function (data) {
        //        console.log("done:");
        //        console.log(data);
        //    });;
        //},

       // childViewContainer: "tbody",
        emptyView: Views.ItemEmptyView,
        collectionEvents: {
            "reset": "render"
        },
        isEmpty: function (collection) {
            return (this.collection.length == 0);
        }


    })
    Views.ItemEmptyView = Marionette.ItemView.extend({
        template: "#row-empty-template"
    })

    //Views.PriceView = Marionette.ItemView.extend({
    //    template: "#price-view-template",
    //    ui: {
    //        'select': 'select'
    //    },
    //    initialize: function () {

    //        console.log('init');
    //    },
    //    tagName: "option",
    //    events: {
    //        "click option": "choosed"
    //    },
    //    choosed: function () {
    //        this.trigger("choosed", "someChoose");
    //        console.log(this.model);
    //    }
    //})
    Views.PricesView = Marionette.ItemView.extend({
        //childView: Views.PriceView,
        //childViewContainer: "select",
        template: "#prices-view-template",
        onBeforeAttach: function () {
            // console.log("before attach");
        },
        collectionEvents: {
            "sync": "render"
        },
        ui: {
            'select': 'select'
        },
        events: {
            "change @ui.select": "selected",
            "focusout .edit": "cancellEdit"
        },
        selected: function (ev) {

            //console.log(ev.currentTarget.value);
            App.trigger("price:selected", ev.currentTarget.value);
        },
        cancellEdit: function () {
            console.log("focusOut");
            App.trigger("price:cancell");
        },

        onRender: function () {
            // console.log("price rendered");
        }
        // emptyView:"#no-data"


    })
    //Views.PriceIVView = Marionette.ItemView.extend({
    //    template: "#priceIV-view-template",
    //    onRender: function () {
    //        console.log("price rendered");
    //    },
    //    collectionEvents: {
    //        "all": "render"
    //    },
    //})
})