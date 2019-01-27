import React, { Component } from "react";

import ReadyView from "../components/ReadyView";
import EggView from "../components/EggView";

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
          <EggView
            airconsole={airconsole}
            ready={() =>
              this.setState({
                ready: true
              })
            }
            text="Tap to Hatch!"
          />
        )}
      </div>
    );
  }
}

export default IntroView;
