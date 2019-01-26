import React, { Component } from "react";
import AirConsole from "air-console";

import egg from "../../assets/images/egg_1.png";

class ReadyView extends Component {
  render() {
    const { airconsole, unready } = this.props;

    return (
      <div>
        <image src={egg} />
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
