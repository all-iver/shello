import React from "react";
import ReactDOM from "react-dom";

import "./styles/main.css";
import Flipper from "./components/Flipper";
import AirConsole from "air-console";

const airconsole = new AirConsole();

ReactDOM.render(
  <div id="app">
    <Flipper airconsole={airconsole} />
    <Flipper airconsole={airconsole} />
  </div>,
  document.getElementById("root")
);
