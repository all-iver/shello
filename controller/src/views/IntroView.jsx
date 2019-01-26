import React, { Component } from "react";
import AirConsole from "air-console";

class IntroView extends Component {
  render() {
    const { airconsole } = this.props;

    return (
      <div id="introView">
        <button
          onClick={() => {
            console.log("ready");
            airconsole.message(AirConsole.SCREEN, {
              action: `ready`
            });
          }}
        >
          Ready
        </button>
        <button
          onClick={() => {
            console.log("unready");
            airconsole.message(AirConsole.SCREEN, {
              action: `unready`
            });
          }}
        >
          Un-Ready
        </button>
      </div>
    );
  }
}

export default IntroView;
