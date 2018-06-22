define([
    "dojo/store/Memory",
    "require"
],
function (
    Memory,
    require
    ) {

    var config = [{
        "id": "GIS"
    }];

    var memory = new Memory({ data: config, idProperty: "id" });
    return memory;
});
