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
    const {
      airconsole,
      body,
      number,
      turtleHidden,
      hideTurtle,
      showBow
    } = this.props;

    return (
      <div id="introView">
        {this.state.ready || number !== "?" ? (
          <ReadyView
            airconsole={airconsole}
            unready={() => {
              hideTurtle();

              this.setState({
                ready: false
              });
            }}
            body={body}
            number={number}
            turtleHidden={turtleHidden}
            showBow={showBow}
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
