import React, { Component, Fragment } from "react";
import AirConsole from "air-console";

import egg from "../../assets/images/egg_1.png";

class IntroView extends Component {
  constructor(props) {
    super(props);

    this.state = {
      ready: false
    };
  }
  render() {
    const { airconsole } = this.props;

    return (
      <div id="introView">
        {this.state.ready ? (
          <div className="unready">
            <label>Click to Un-Ready</label>
            <button
              onClick={() => {
                this.setState({
                  ready: false
                });

                airconsole.message(AirConsole.SCREEN, {
                  action: "unready"
                });
              }}
            >
              X
            </button>
          </div>
        ) : (
          <button
            onClick={() => {
              this.setState({
                ready: true
              });

              airconsole.message(AirConsole.SCREEN, {
                action: "ready"
              });
            }}
            className="ready"
          >
            Click to Ready!
          </button>
        )}
      </div>
    );
  }
}

export default IntroView;
