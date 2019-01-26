import React from "react";
import ReactDOM from "react-dom";

import "./styles/main.css";
import Flipper from "./components/Flipper";
import AirConsole from "air-console";

const airconsole = new AirConsole({
  orientation: AirConsole.ORIENTATION_LANDSCAPE
});

ReactDOM.render(
  <div id="app">
    <Flipper airconsole={airconsole} side="Left" />
    <Flipper airconsole={airconsole} side="Right" />
  </div>,
  document.getElementById("root")
);
