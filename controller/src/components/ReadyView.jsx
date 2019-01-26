import React, { Component } from "react";
import AirConsole from "air-console";

import Egg from "./Egg";

class ReadyView extends Component {
  render() {
    const { airconsole, unready } = this.props;

    return (
      <div id="readyView">
        <Egg />
        <div className="unready">
          <label>Click to Un-Ready</label>
          <button
            onClick={() => {
              unready();

              airconsole.message(AirConsole.SCREEN, {
                action: "unready"
              });
            }}
          >
            X
          </button>
        </div>
      </div>
    );
  }
}

export default ReadyView;
