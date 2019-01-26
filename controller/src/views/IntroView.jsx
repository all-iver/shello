import React, { Component } from "react";
import AirConsole from "air-console";

import ReadyView from "../components/ReadyView";

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
          <ReadyView
            airconsole={airconsole}
            unready={() =>
              this.setState({
                ready: false
              })
            }
          />
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
