import React, { Component } from "react";
import AirConsole from "air-console";

import Egg from "./Egg";

class EggView extends Component {
  render() {
    const { airconsole, ready } = this.props;
    return (
      <div id="eggView">
        <Egg
          ready={() => {
            ready();

            airconsole.message(AirConsole.SCREEN, {
              action: "ready"
            });
          }}
        />
        <p>Tap to Hatch!</p>
      </div>
    );
  }
}

export default EggView;
