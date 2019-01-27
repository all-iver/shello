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
    const { airconsole } = this.props;

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
          text="Waiting for other players to finished the race!"
        />
      </div>
    );
  }
}

export default IntroView;
