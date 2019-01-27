import React, { Component } from "react";
import AirConsole from "air-console";

import Turtle from "./Turtle";

class ReadyView extends Component {
  render() {
    const { airconsole, unready } = this.props;

    return (
      <div id="readyView">
        <p>Waiting for the game to start!</p>
        <Turtle showSwipeArrows={false} />
        <div className="unready">
          <label>Click to Un-Hatch</label>
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
