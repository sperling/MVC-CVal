(function ($) {

    // taken from jquery.validate.unobtrusive.js
    function getModelPrefix(fieldName) {
        return fieldName.substr(0, fieldName.lastIndexOf(".") + 1);
    }

    // taken from jquery.validate.unobtrusive.js
    function appendModelPrefix(value, prefix) {
        if (value.indexOf("*.") === 0) {
            value = value.replace("*.", prefix);
        }
        return value;
    }

    function Init(options, ruleName) {
        var prefix = getModelPrefix(options.element.name),
            other = options.params.conditionproperty,
            fullOtherName = appendModelPrefix(other, prefix),
            element = $(options.form).find(":input[name=" + fullOtherName + "]")[0],
            ifnot = (/^true$/i).test(options.params.validateifnot);

        options.rules[ruleName] = [element, ifnot];

        if (options.message) {
            options.messages[ruleName] = options.message;
        }

        $(element).change(function () {
            // run validation for element again if condition is not true anymore.
            // hide error message.
            // TODO:    make sure it works with radiobutton, select.
            if ($(this).is(':checked') === ifnot) {
                $(options.form).validate().element(options.element);
            }
        });
    }

    function Validate(conditionProperty, validateIfNot, callback) {
        var condition = !validateIfNot;

        // TODO:    make sure it works with radiobutton, select.
        if ($(conditionProperty).is(':checked') === condition) {
            return callback();
        }
        else {
            return true;
        }
    }

    jQuery.validator.addMethod("cvrequired", function (value, element, params) {
        var that = this;
        return Validate(params[0], params[1], function () {
            return jQuery.validator.methods['required'].call(that, value, element, true);
        });
    });

    jQuery.validator.unobtrusive.adapters.add('cvrequired', ['conditionproperty', 'validateifnot'], function (options) {
        Init(options, 'cvrequired');
    });

} (jQuery));