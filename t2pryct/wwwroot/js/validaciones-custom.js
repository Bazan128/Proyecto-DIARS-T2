(function ($) {
    if (!$ || !$.validator) { return; }

    function normalizar(valor) {
        var limpio = (valor || "").replace(/[\s\-\(\)]/g, "");
        if (limpio.indexOf("+51") === 0) { limpio = limpio.substring(3); }
        return limpio;
    }

    $.validator.addMethod("telefonoperuano", function (value) {
        if (!value) { return true; }
        return /^9\d{8}$/.test(normalizar(value));
    });

    $.validator.unobtrusive.adapters.addBool("telefonoperuano");
})(window.jQuery);
