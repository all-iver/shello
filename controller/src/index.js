import React from "react";
import ReactDOM from "react-dom";

import "./styles/main.css";
import PlayView from "./views/PlayView";
import AirConsole from "air-console";

const airconsole = new AirConsole({
  orientation: AirConsole.ORIENTATION_LANDSCAPE
});

ReactDOM.render(
  <div id="app">
    <PlayView airconsole={airconsole} />
  </div>,
  document.getElementById("root")
);
