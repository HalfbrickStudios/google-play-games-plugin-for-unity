#!/usr/bin/env bash

# set -x

__dirname="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

version='2.0.0'
extract="$__dirname/GooglePlayGamesPlugin-${version}"
restore="$__dirname/com.google.play.games-${version}"

# Clean and extract Unity package
! test -d $extract || rm -rf $extract
mkdir -p $extract
tar --no-xattrs -zxf $extract.unitypackage -C $extract --warning=no-unknown-keyword || tar zxf $extract.unitypackage -C $extract

# Clean and restore Unity package
! test -d $restore || rm -rf $restore
mkdir -p $restore
for it in $extract/*/; do
    dir="${it%/}"
    if ! test -f "$dir/pathname"; then
        echo ">> Skipping non-indexed asset: $dir" >&2
        continue
    fi
    item="$(cat "$dir/pathname")"
    if test "$item" == ""; then
        echo ">> Skipping empty-path asset: $dir" >&2
        continue
    fi
    container="$(dirname "$item")"
    mkdir -p "$restore/$container"
    if test -f "$dir/asset"; then
        cp "$dir/asset" "$restore/$item"
    fi
    if test -f "$dir/asset.meta"; then
        cp "$dir/asset.meta" "$restore/$item.meta"
    fi
done

# Copy the package definition
cp "$__dirname/../Assets/Public/GooglePlayGames/com.google.play.games/package.json" "$restore"
