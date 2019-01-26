import React from "react";
import ReactDOM from "react-dom";

import "./styles/main.css";
import AirConsole from "air-console";
import App from "./views/App";

const airconsole = new AirConsole({
  orientation: AirConsole.ORIENTATION_LANDSCAPE
});

ReactDOM.render(
  <App airconsole={airconsole} />,
  document.getElementById("root")
);
