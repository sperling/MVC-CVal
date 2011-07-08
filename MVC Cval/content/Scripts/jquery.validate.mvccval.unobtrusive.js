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

    function Init(options, ruleName, params) {
        var prefix = getModelPrefix(options.element.name),
            conditionProperty = options.params.conditionproperty,
            fullConditionName = appendModelPrefix(conditionProperty, prefix),
            conditionElement = $(options.form).find(":input[name=" + fullConditionName + "]")[0],
            ifnot = (/^true$/i).test(options.params.validateifnot);
        var i;

        options.rules[ruleName] = [conditionElement, ifnot];

        if (params !== undefined) {
            // add remaning parameters needed for validation function.
            for (i = 0; i < params.length; i++) {
                options.rules[ruleName].push(params[i]);
            }
        }

        if (options.message) {
            options.messages[ruleName] = options.message;
        }

        $(conditionElement).change(function () {
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

    function ApplyParams(params) {
        var commonParams = ['conditionproperty', 'validateifnot'];
        params = params || [];

        //commonParams.push.apply(commonParams, params);

        //return commonParams;
        return commonParams.concat(params);
    }

    jQuery.validator.addMethod("cvrequired", function (value, element, params) {
        var that = this;
        return Validate(params[0], params[1], function () {
            return jQuery.validator.methods['required'].call(that, value, element, true);
        });
    });

    jQuery.validator.unobtrusive.adapters.add('cvrequired', ApplyParams(), function (options) {
        Init(options, 'cvrequired');
    });

    jQuery.validator.addMethod("cvrangelength", function (value, element, params) {
        var that = this;
        return Validate(params[0], params[1], function () {
            return jQuery.validator.methods['rangelength'].call(that, value, element, params.slice(2));
        });
    });

    jQuery.validator.addMethod("cvlength", function (value, element, params) {
        var that = this;
        return Validate(params[0], params[1], function () {
            if (params[2] === 'min') {
                return jQuery.validator.methods['minlength'].call(that, value, element, params[3]);
            }
            else if (params[2] === 'max') {
                return jQuery.validator.methods['maxlength'].call(that, value, element, params[3]);
            }
            else {
                throw "Bad argument for 'cvlength'";
            }
        });
    });

    jQuery.validator.unobtrusive.adapters.add('cvlength', ApplyParams(['min', 'max']), function (options) {
        // from adapters.addMinMax
        // have three rules here.
        var min = options.params.min,
            max = options.params.max;

        if (min && max) {
            Init(options, 'cvrangelength', [min, max]);
        }
        else if (min) {
            Init(options, 'cvlength', ['min', min]);
        }
        else if (max) {
            Init(options, 'cvlength', ['max', max]);
        }
    });

} (jQuery));