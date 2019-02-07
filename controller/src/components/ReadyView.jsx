import React, { Component } from "react";
import AirConsole from "air-console";

import Turtle from "./Turtle";

class ReadyView extends Component {
  render() {
    const {
      airconsole,
      unready,
      body,
      number,
      turtleHidden,
      showBow
    } = this.props;

    return (
      <div id="readyView">
        <Turtle
          showSwipeArrows={false}
          body={body}
          number={number}
          hidden={turtleHidden}
          showBow={showBow}
        />
        <p>This is your turtle. Look for them in the nest!</p>
        {/* <div className="unready">
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
        </div> */}
      </div>
    );
  }
}

export default ReadyView;
