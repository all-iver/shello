import React, { Component } from "react";
import AirConsole from "air-console";

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
        <button
          onClick={() => {
            this.setState({
              ready: !this.state.ready
            });

            airconsole.message(AirConsole.SCREEN, {
              action: this.state.ready ? "unready" : "ready"
            });
          }}
          className={this.state.ready ? "ready" : "unready"}
        >
          Click to {this.state.ready ? "Un-Ready" : "Ready"}!
        </button>
      </div>
    );
  }
}

export default IntroView;
