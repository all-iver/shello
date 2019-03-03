import React, { Component } from "react";

import EggView from "../components/EggView";

class IntroView extends Component {
  constructor(props) {
    super(props);

    this.state = {
      ready: false
    };
  }
  render() {
    const { airconsole, hasAnyMessageBeenReceived } = this.props;

    return (
      <div id="waitingView">
        <EggView
          airconsole={airconsole}
          ready={() =>
            this.setState({
              ready: true
            })
          }
          crackable={false}
          text={
            hasAnyMessageBeenReceived
              ? "Waiting for other players to finish the race!"
              : "Waiting for game to load..."
          }
        />
      </div>
    );
  }
}

export default IntroView;
