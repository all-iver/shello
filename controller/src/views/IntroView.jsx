import React, { Component } from "react";

import ReadyView from "../components/ReadyView";
import EggView from "../components/EggView";
import sand from "../../assets/images/sand.png";

class IntroView extends Component {
  constructor(props) {
    super(props);

    this.state = {
      ready: false
    };
  }
  render() {
    const { airconsole, body, number } = this.props;

    return (
      <div id="introView">
        <img src={sand} />
        {this.state.ready ? (
          <ReadyView
            airconsole={airconsole}
            unready={() =>
              this.setState({
                ready: false
              })
            }
            body={body}
            number={number}
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
