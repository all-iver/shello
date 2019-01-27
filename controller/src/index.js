import React from "react";
import ReactDOM from "react-dom";
import WebFont from "webfontloader";
import { disableBodyScroll } from "body-scroll-lock";

import "./styles/main.css";
import "./styles/wobble.css";
import "./styles/slideOutUp.css";
import AirConsole from "air-console";
import App from "./views/App";

disableBodyScroll(document.querySelector("#root"));

WebFont.load({
  google: {
    families: ["Bungee Inline", "Londrina Solid"]
  }
});

const airconsole = new AirConsole({
  orientation: AirConsole.ORIENTATION_LANDSCAPE
});

ReactDOM.render(
  <App airconsole={airconsole} />,
  document.getElementById("root")
);
