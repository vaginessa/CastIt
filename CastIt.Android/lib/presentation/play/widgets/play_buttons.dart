import 'package:castit/application/bloc.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

class PlayButtons extends StatelessWidget {
  static const double _iconSize = 50;

  final bool areDisabled;

  const PlayButtons({
    Key? key,
    this.areDisabled = false,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final isDarkTheme = theme.brightness == Brightness.dark;
    final iconColor = isDarkTheme ? Colors.white : Colors.black;
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 15),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: <Widget>[
          IconButton(
            iconSize: _iconSize,
            padding: EdgeInsets.zero,
            onPressed: areDisabled ? null : () => _skipThirtySeconds(context, false),
            icon: Icon(Icons.fast_rewind, color: iconColor),
          ),
          IconButton(
            iconSize: _iconSize,
            padding: EdgeInsets.zero,
            onPressed: areDisabled ? null : () => _goTo(context, false, true),
            icon: Icon(Icons.skip_previous, color: iconColor),
          ),
          Container(
            decoration: BoxDecoration(
              color: theme.colorScheme.secondary,
              borderRadius: BorderRadius.circular(50.0),
            ),
            child: BlocBuilder<PlayBloc, PlayState>(
              builder: (ctx, state) => state.maybeMap(
                playing: (state) => _buildPlayBackButton(context, state.isPaused!),
                orElse: () => _buildPlayBackButton(context, false),
              ),
            ),
          ),
          IconButton(
            iconSize: _iconSize,
            padding: EdgeInsets.zero,
            onPressed: areDisabled ? null : () => _goTo(context, true, false),
            icon: Icon(Icons.skip_next, color: iconColor),
          ),
          IconButton(
            iconSize: _iconSize,
            padding: EdgeInsets.zero,
            onPressed: areDisabled ? null : () => _skipThirtySeconds(context, true),
            icon: Icon(Icons.fast_forward, color: iconColor),
          ),
        ],
      ),
    );
  }

  IconButton _buildPlayBackButton(BuildContext ctx, bool isPaused) {
    return IconButton(
      iconSize: 65,
      onPressed: areDisabled ? null : () => _togglePlayBack(ctx),
      icon: Icon(
        isPaused ? Icons.pause : Icons.play_arrow,
        color: Colors.white,
      ),
    );
  }

  void _togglePlayBack(BuildContext ctx) {
    final bloc = ctx.read<ServerWsBloc>();
    bloc.togglePlayBack();
  }

  void _goTo(BuildContext ctx, bool next, bool previous) {
    final bloc = ctx.read<ServerWsBloc>();
    bloc.goTo(next: next, previous: previous);
  }

  void _skipThirtySeconds(BuildContext ctx, bool forward) {
    final seconds = 30.0 * (forward ? 1 : -1);
    final bloc = ctx.read<ServerWsBloc>();
    bloc.skipSeconds(seconds);
  }
}
