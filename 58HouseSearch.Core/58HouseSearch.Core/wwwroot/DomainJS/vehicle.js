define(function() {
    var _type = "SUBWAY,BUS";
    return {
        get type() {
            return _type;
        },
        set type(value) {
            _type = value;
        }
    }
});