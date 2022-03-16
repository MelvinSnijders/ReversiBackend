"use strict";

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); Object.defineProperty(Constructor, "prototype", { writable: false }); return Constructor; }

/**
 * Widget for showing feedback alerts in the browser.
 */
var FeedbackWidget = /*#__PURE__*/function () {
  /**
   * Construct a new widget.
   * @param elementId The element of the alert component.
   */
  function FeedbackWidget(elementId) {
    _classCallCheck(this, FeedbackWidget);

    this._elementId = elementId;
  }
  /**
   * The element to which to bind the alert to.
   * @returns {*} The id of the element.
   */


  _createClass(FeedbackWidget, [{
    key: "elementId",
    get: function get() {
      return this._elementId;
    }
    /**
     * Show an alert message in the browser.
     * @param message The content of the alert.
     * @param type The type of alert (success|danger), default is danger.
     * @param skipLog Whether to skip logging the alert in the local storage.
     */

  }, {
    key: "show",
    value: function show(message, type, skipLog) {
      var div = $("#".concat(this.elementId));
      div.css("display", div.css("display") === "none" && "block");
      div.text(message);

      switch (type) {
        case "success":
          div.addClass("alert-success");
          break;

        case "danger":
        default:
          div.addClass("alert-danger");
          break;
      }

      if (!skipLog) this.log({
        message: message,
        type: type
      });
    }
    /**
     * Hide the alert from the browser.
     */

  }, {
    key: "hide",
    value: function hide() {
      var div = $("#".concat(this.elementId));
      div.css("display", div.css("display") === "block" && "none");
    }
    /**
     * Log a message to the local storage.
     * The local storage holds the latest 10 alerts.
     * @param message The message to store (JSON object with message and type).
     */

  }, {
    key: "log",
    value: function log(message) {
      var _localStorage$feedbac;

      var logArray = JSON.parse((_localStorage$feedbac = localStorage.feedback_widget) !== null && _localStorage$feedbac !== void 0 ? _localStorage$feedbac : "[]");
      if (logArray.length >= 10) logArray.pop();
      logArray.unshift(message);
      localStorage.feedback_widget = JSON.stringify(logArray);
    }
    /**
     * Clear the local storage logs.
     */

  }, {
    key: "removeLog",
    value: function removeLog() {
      localStorage.removeItem("feedback_widget");
    }
    /**
     * Retrieve the complete log (max 10 items), format them and show them in the browser.
     */

  }, {
    key: "history",
    value: function history() {
      var _localStorage$feedbac2;

      var logArray = JSON.parse((_localStorage$feedbac2 = localStorage.feedback_widget) !== null && _localStorage$feedbac2 !== void 0 ? _localStorage$feedbac2 : "[]"); // <type |success|error|>  -  <berichttekst> <\n>

      var formatted = logArray.map(function (d) {
        return "".concat(d.type, " - ").concat(d.message);
      }).join("\n");
      this.show(formatted, "success", true);
    }
  }]);

  return FeedbackWidget;
}();

var apiUrl = 'url/super/duper/game';

var Game = function (url) {
  var configMap = {
    apiUrl: url
  };
  var stateMap = {
    gameState: 0
  };

  var _getCurrentGameState = function _getCurrentGameState() {
    Game.Model.getGameState().then(function (d) {
      return stateMap.gameState = d;
    });
  };

  var privateInit = function privateInit(afterInit) {
    console.log(configMap.apiUrl);
    setInterval(_getCurrentGameState, 2000);
    afterInit();
  }; // Waarde/object geretourneerd aan de outer scope


  return {
    init: privateInit
  };
}(apiUrl);

Game.Data = function () {
  var configMap = {
    mock: [{
      url: "api/Spel/Beurt",
      data: 0
    }],
    apiKey: '2bce53e0f162422c7108493ce597d291'
  };
  var stateMap = {
    environment: 'development'
  };

  var getMockData = function getMockData(url) {
    var mockData = configMap.mock;
    return new Promise(function (resolve, reject) {
      resolve(mockData);
    });
  };

  var get = function get(url) {
    if (stateMap.environment === "production") {
      return $.get(url)["catch"](function (e) {
        console.log(e.message);
      });
    }

    if (stateMap.environment === "development") {
      return getMockData(url);
    }
  };

  var privateInit = function privateInit(environment) {
    console.log("Init vanuit Data!");

    if (environment !== "development" && environment !== "production") {
      throw new Error("Environment must be production or development.");
    }

    stateMap.environment = environment;
  };

  return {
    init: privateInit,
    get: get
  };
}();

Game.Model = function () {
  var configMap = {};

  var privateInit = function privateInit() {
    console.log("Init vanuit Model!");
  };

  var getWeather = function getWeather() {
    Game.Data.get('http://api.openweathermap.org/data/2.5/weather?q=zwolle&apikey=2bce53e0f162422c7108493ce597d291').then(function (data) {
      if (!data.weather[0].temp) throw Error("No temperature in result");
    });
  };

  var _getGameState = function _getGameState() {
    var stateData = Game.Data.get('api/Spel/Beurt/<token>');
    return stateData.then(function (d) {
      if (d < 0 || d > 2) {
        throw Error("Game state is not valid");
      }

      return d;
    });
  };

  return {
    init: privateInit,
    getWeather: getWeather,
    getGameState: _getGameState
  };
}();

Game.Reversi = function () {
  var configMap = {};

  var privateInit = function privateInit() {
    console.log("Init vanuit Reversi!");
  };

  return {
    init: privateInit
  };
}();