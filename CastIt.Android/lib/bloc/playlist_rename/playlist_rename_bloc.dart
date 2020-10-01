import 'dart:async';

import 'package:bloc/bloc.dart';
import 'package:freezed_annotation/freezed_annotation.dart';

import '../../common/extensions/string_extensions.dart';

part 'playlist_rename_bloc.freezed.dart';
part 'playlist_rename_event.dart';
part 'playlist_rename_state.dart';

class PlayListRenameBloc extends Bloc<PlayListRenameEvent, PlayListRenameState> {
  PlayListRenameBloc() : super(PlayListRenameState.initial());

  PlayListRenameLoadedState get currentState => state as PlayListRenameLoadedState;

  @override
  Stream<PlayListRenameState> mapEventToState(
    PlayListRenameEvent event,
  ) async* {
    if (event is! PlayListNameChangedEvent) {
      yield PlayListRenameState.initial();
    }

    final s = event.map(
      load: (e) => PlayListRenameState.loaded(currentName: e.name, isNameValid: _isNameValid(e.name)),
      nameChanged: (e) => currentState.copyWith(isNameValid: _isNameValid(e.name)),
    );

    yield s;
  }

  bool _isNameValid(String name) => !name.isNullEmptyOrWhitespace && !name.isLengthValid(minLength: 1);
}
